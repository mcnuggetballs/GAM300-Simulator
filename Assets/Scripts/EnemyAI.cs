using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] protected Transform player;
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
    protected bool _playerDetected;
    Animator animator;

    public Vector3 GetCurrentPlayerPos()
    {
        return player.transform.position;
    }
    protected enum State
    {
        Patrolling,
        Chasing,
        Attacking
    }

    protected State _currentState;

    protected void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null )
        {
            animator.SetBool("CanStun", true);
        }
        theEntity = GetComponent<Entity>();
        _currentState = State.Patrolling;
        _timeSinceLastAttack = attackCooldown;
        _timeSinceLastPatrol = 0f;
    }

    protected void Update()
    {
        _timeSinceLastAttack += Time.deltaTime;

        switch (_currentState)
        {
            case State.Patrolling:
                Patrol();
                break;
            case State.Chasing:
                Chase();
                break;
            case State.Attacking:
                Attack();
                break;
        }
    }

    protected void Aggro()
    {
        _playerDetected = true;
        _lastKnownPlayerPosition = player.position;
        _currentState = State.Chasing;
    }
    protected void Patrol()
    {
        if (_playerDetected = DetectPlayer())
        {
            _currentState = State.Chasing;
        }
        else
        {
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
        }
    }

    protected void Chase()
    {
        if (HasLineOfSight())
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _lastKnownPlayerPosition);

            // Stop the enemy at the stopping distance instead of moving all the way to the player
            if (distanceToPlayer > stoppingDistance)
            {
                // Calculate a position closer to the player but within the stopping distance
                Vector3 directionToPlayer = (_lastKnownPlayerPosition - transform.position).normalized;
                Vector3 targetPosition = _lastKnownPlayerPosition - directionToPlayer * stoppingDistance;

                SetDestinationAndPathfinding(targetPosition);
            }

            // Switch to attacking if within attack range
            if (distanceToPlayer <= attackRadius)
            {
                _currentState = State.Attacking;
            }
        }
        else
        {
            _currentState = State.Patrolling;
        }
    }

    protected void Attack()
    {
        if (GetComponent<EnemyControllerRB>() != null)
        {
            GetComponent<EnemyControllerRB>().SetLookDirection((player.position - transform.position).normalized);
        }
        if (_timeSinceLastAttack >= attackCooldown)
        {
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
        }
    }

    protected bool DetectPlayer()
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

    protected bool HasLineOfSight()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        if (!Physics.Raycast(transform.position, directionToPlayer, Vector3.Distance(transform.position, player.position), obstacleLayer))
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
