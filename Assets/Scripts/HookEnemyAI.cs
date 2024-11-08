using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HookEnemyAI : EnemyAI
{
    Animator animator;
    public bool hasPulled = false;
    public bool switchState = false;
    public bool complete = false;
    EnemyControllerRB enemyControllerRB;
    bool shotOut = false;
    [SerializeField]
    ExpressionDisplayer expression;
    float shootDelay = 1.5f;
    float timer = 0.0f;
    float idleTime = 2.0f;
    float idleTimer = 0.0f;
    [SerializeField]
    float smackDistance;

    private void Awake()
    {
        enemyControllerRB= GetComponent<EnemyControllerRB>();
        animator = GetComponent<Animator>();
    }
    protected override void Update()
    {
        switch (_currentState)
        {
            case State.Patrolling:
                GetComponent<PathfindingScript>().jumpingNavLinkEnabled = false;
                Patrol();
                break;
            case State.Idle:
                idleTimer += Time.deltaTime;
                if (idleTimer >= idleTime)
                {
                    idleTimer = 0.0f;
                    _currentState = State.Chasing;
                }
                else
                {
                    GetComponent<EnemyControllerRB>().disableMovement = true;
                    GetComponent<EnemyControllerRB>().StopMovement();
                }
                break;
            case State.Chasing:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Hurt"))
                {
                    _timeSinceLastAttack += Time.deltaTime;
                }
                GetComponent<PathfindingScript>().jumpingNavLinkEnabled = true;
                Chase();
                break;
            case State.Attacking:
                GetComponent<PathfindingScript>().jumpingNavLinkEnabled = true;
                Attack();
                break;
        }
    }

    protected override void Attack()
    {
        timer += Time.deltaTime;

        if (timer >= shootDelay)
        {
            Vector3 mypos = transform.position;
            if (transform.position.y - player.position.y <= 4.0f)
            {
                mypos.y = player.position.y;
            }
            float distanceToPlayer = Vector3.Distance(mypos, player.position);
            if (distanceToPlayer <= smackDistance)
            {
                timer = 0.0f;
                GetComponent<EnemyControllerRB>().disableMovement = false;
                StartCoroutine(SwingAttack());
                hasPulled = false;
                switchState = false;
                _timeSinceLastAttack = 0f;
                _currentState = State.Idle;
                idleTimer = 0.0f;
                complete = false;
                shotOut = false;
                if (animator != null)
                {
                    animator.SetBool("CanStun", true);
                }
                expression.Hide();
                return;
            }
        }

        if (animator != null)
        {
            animator.SetBool("CanStun", false);
        }
        if (hasPulled == false && complete == false && shotOut == false)
        {
            enemyControllerRB.SetLookDirection((player.position - transform.position).normalized);
            switchState = false;
            complete = false;
            if (timer >= shootDelay)
            {
                expression.Hide();
                if (GetComponent<EnemyControllerRB>() != null && shotOut == false)
                {
                    GetComponent<EnemyControllerRB>().SetLookDirection((player.position - transform.position).normalized);
                }
                // Attack logic here
                if (theEntity)
                {
                    if (theEntity.skill)
                    {
                        shotOut = true;
                        theEntity.skill.Activate(gameObject);
                    }
                }
            }
        }
        if (hasPulled && complete == false)
        {
            if (animator != null)
            {
                switchState = false;
                StartCoroutine(SwingAttack());
                hasPulled = false;
                complete = true;
            }
        }
        if (switchState)
        {
            timer = 0.0f;
            hasPulled = false;
            switchState = false;
            _timeSinceLastAttack = 0f;
            _currentState = State.Idle;
            idleTimer = 0.0f;
            complete = false;
            shotOut = false;
            if (animator != null)
            {
                animator.SetBool("CanStun", true);
            }
        }
    }

    IEnumerator SwingAttack()
    {
        while(!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle Walk Run Blend"))
        {
            yield return null;
        }
        if (animator != null)
        {
            animator.SetBool("Hit1", true);
        }
        switchState = true;
        yield return null;
    }

    protected override void Chase()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Hurt"))
        {
            return;
        }
        Vector3 mypos = transform.position;
        if (transform.position.y - player.position.y <= 4.0f)
        {
            mypos.y = player.position.y;
        }
        float distanceToPlayer = Vector3.Distance(mypos, player.position);

        if (HasLineOfSight())
        {
            // If the player is within attack range, move away from them

            if (distanceToPlayer > stoppingDistance)
            {
                // Otherwise, continue to chase the player but stop within stopping distance
                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                GetComponent<EnemyControllerRB>().disableMovement = false;
                SetDestinationAndPathfinding(player.position);
            }
            else
            {
                GetComponent<EnemyControllerRB>().disableMovement = true;
                GetComponent<EnemyControllerRB>().StopMovement();
            }

            if (_timeSinceLastAttack >= attackCooldown)
            {
                timer = 0.0f;
                if (distanceToPlayer <= attackRadius)
                {
                    _currentState = State.Attacking;
                    expression.Show();
                    GetComponent<EnemyControllerRB>().disableMovement = true;
                    GetComponent<EnemyControllerRB>().StopMovement();
                }
            }
        }
        else
        {
            _currentState = State.Patrolling;
        }
    }

    private void MoveAwayFromPlayer()
    {
        // Calculate the direction away from the player
        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;

        // Set a destination away from the player at a certain distance (e.g., 5 units away)
        Vector3 newTargetPosition = transform.position + directionAwayFromPlayer * 5f;

        // Use pathfinding to move to the new position
        SetDestinationAndPathfinding(newTargetPosition);
    }
}
