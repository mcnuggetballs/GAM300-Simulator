using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    protected Transform player;
    [SerializeField] protected PathfindingScript pathfinding;
    [SerializeField] protected float detectionRadius = 10f;
    [SerializeField] protected float attackRadius = 2f;
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected LayerMask obstacleLayer;
    [SerializeField] protected float patrolWaitTime = 2f;
    [SerializeField] protected float attackCooldown = 1f;
    [SerializeField] protected float stoppingDistance = 0.0f;
    protected Entity theEntity;

    protected Vector3 _lastKnownPlayerPosition;
    protected float _timeSinceLastAttack;
    protected float _timeSinceLastPatrol;
    protected float patrolSoundTime;
    protected bool _playerDetected;
    Animator animator;
    protected Vector3 oldPos;

    public Vector3 GetCurrentPlayerPos()
    {
        return player.transform.position;
    }
    public Transform GetCurrentPlayerTransform()
    {
        return player.transform;
    }
    public Vector3 GetCurrentPlayerNeckPos()
    {
        return player.GetComponent<Entity>().neck.position;
    }
    protected enum State
    {
        Idle,
        Patrolling,
        Chasing,
        Attacking
    }

    protected State _currentState;

    protected virtual void Start()
    {
        Objective1Manager.Instance.totalEnemies++;
        Objective1Manager.Instance.currentEnemies++;
        ThirdPersonControllerRB pp = FindObjectOfType<ThirdPersonControllerRB>();
        if (pp != null)
        {
            player = pp.transform;
        }
        oldPos = transform.position;
        animator = GetComponent<Animator>();
        if (animator != null )
        {
            animator.SetBool("CanStun", true);
        }
        theEntity = GetComponent<Entity>();
        _currentState = State.Patrolling;
        _timeSinceLastAttack = attackCooldown;
        _timeSinceLastPatrol = 0f;
        patrolSoundTime = 0f;
    }

    protected virtual void Update()
    {
        _timeSinceLastAttack += Time.deltaTime;

        switch (_currentState)
        {
            case State.Patrolling:
                GetComponent<PathfindingScript>().jumpingNavLinkEnabled = false;
                Patrol();
                break;
            case State.Chasing:
                GetComponent<PathfindingScript>().jumpingNavLinkEnabled = true;
                Chase();
                break;
            case State.Attacking:
                GetComponent<PathfindingScript>().jumpingNavLinkEnabled = true;
                Attack();
                break;
        }
    }

    public virtual void Aggro()
    {
        _playerDetected = true;
        _lastKnownPlayerPosition = player.position;
        _currentState = State.Chasing;
        AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.EnemyAggroSounds[Random.Range(0, AudioManager.instance.EnemyAggroSounds.Length)], transform.position);
    }
    protected virtual void Patrol()
    {
        if (_playerDetected = DetectPlayer())
        {
            _currentState = State.Chasing;
            AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.EnemyAggroSounds[Random.Range(0, AudioManager.instance.EnemyAggroSounds.Length)],transform.position);
        }
        else
        {
            patrolSoundTime += Time.deltaTime;
            if (patrolSoundTime >= 0.5f)
            {
                AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.EnemyPatrolSounds[Random.Range(0, AudioManager.instance.EnemyPatrolSounds.Length)], transform.position);
                patrolSoundTime = 0f;
            }
            if (_timeSinceLastPatrol >= patrolWaitTime)
            {
                Vector3 randomDestination = GetRandomNavMeshPosition(transform.position, 10f);
                SetDestinationAndPathfinding(randomDestination);
                _timeSinceLastPatrol = 0f;
            }
            else
            {
                if (!pathfinding.IsMoving())
                    _timeSinceLastPatrol += Time.deltaTime;
            }
            if (GetComponent<NavMeshAgent>().isOnOffMeshLink && !GetComponent<PathfindingScript>().jumpingNavLinkEnabled)
            {
                transform.position = oldPos;
                GetComponent<NavMeshAgent>().Warp(oldPos);
                Debug.Log(gameObject.name);
                SetDestinationAndPathfinding(oldPos);
                return;
            }
            else
            {
                oldPos = transform.position;
            }
        }
    }

    protected virtual void Chase()
    {
        if (HasLineOfSight())
        {
            Vector3 mypos = transform.position;
            if (transform.position.y - _lastKnownPlayerPosition.y <= 4.0f)
            {
                mypos.y = _lastKnownPlayerPosition.y;
            }
            float distanceToPlayer = Vector3.Distance(mypos, _lastKnownPlayerPosition);

            // Stop the enemy at the stopping distance instead of moving all the way to the player
            if (distanceToPlayer > stoppingDistance)
            {
                // Calculate a position closer to the player but within the stopping distance
                Vector3 directionToPlayer = (_lastKnownPlayerPosition - transform.position).normalized;

                GetComponent<EnemyControllerRB>().disableMovement = false;
                SetDestinationAndPathfinding(_lastKnownPlayerPosition);
            }
            else
            {
                GetComponent<EnemyControllerRB>().disableMovement = true;
                GetComponent<EnemyControllerRB>().StopMovement();
            }

            // Switch to attacking if within attack range
            if (distanceToPlayer <= attackRadius)
            {
                if (GetComponent<EnemyControllerRB>() != null)
                {
                    GetComponent<EnemyControllerRB>().SetLookDirection((player.position - transform.position).normalized);
                }

                if (_timeSinceLastAttack >= attackCooldown)
                {
                    _currentState = State.Attacking;
                }
            }
        }
        else
        {
            _currentState = State.Patrolling;
        }
    }

    protected virtual void Attack()
    {
        if (GetComponent<PathfindingScript>() != null)
        {
            GetComponent<PathfindingScript>().jumpingNavLinkEnabled = false;
            if (GetComponent<PathfindingScript>().isJumping)
            {
                return;
            }
        }
        if (GetComponent<EnemyControllerRB>() != null)
        {
            GetComponent<EnemyControllerRB>().SetLookDirection((player.position - transform.position).normalized);
        }
        if (theEntity.GetHealthFraction() <= 0.5f)
        {
            if (animator != null)
            {
                animator.SetBool("CanStun", false);
            }
        }
        // Attack logic here
        if (theEntity)
        {
            if (theEntity.skill)
                theEntity.skill.Activate(gameObject);
        }

        _timeSinceLastAttack = 0f;
        _currentState = State.Chasing;
        GetComponent<PathfindingScript>().jumpingNavLinkEnabled = true;
    }

    protected virtual bool DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        foreach (var hit in hits)
        {
            if (hit.transform == player)
            {
                if (HasLineOfSight())
                {
                    _lastKnownPlayerPosition = player.position;
                    return true;
                }
            }
        }
        return false;
    }

    protected virtual bool HasLineOfSight()
    {
        Vector3 directionToPlayer = (player.GetComponent<Entity>().neck.position - GetComponent<Entity>().neck.position).normalized;
        if (!Physics.Raycast(GetComponent<Entity>().neck.position, directionToPlayer, Vector3.Distance(GetComponent<Entity>().neck.position, player.GetComponent<Entity>().neck.position), obstacleLayer))
        {
            _lastKnownPlayerPosition = player.position;
            return true;
        }
        return false;
    }

    protected Vector3 GetRandomNavMeshPosition(Vector3 origin, float distance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, distance, 1);
        return hit.position;
    }

    // New Method to enforce the correct order
    protected void SetDestinationAndPathfinding(Vector3 targetPosition)
    {
        // Ensures that SetDestination is called first, then pathfinding
        if (pathfinding != null)
        {
            pathfinding.FindPath(targetPosition);
        }
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}
