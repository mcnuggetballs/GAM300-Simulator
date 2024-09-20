using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlamAI : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRadius = 10f;
    public float fieldOfViewAngle = 120f;
    public LayerMask playerLayer;
    public LayerMask obstructionLayer;

    [Header("Attack Settings")]
    public float attackRange = 2f;         // The range within which the enemy will attack
    private Animator animator;             // Reference to the Animator
    public float attackCooldown = 1f;      // Time to wait after attacking before moving again
    private bool isInAttackCooldown = false; // Flag to track if the enemy is in attack cooldown
    private float attackCooldownTimer = 0f;  // Timer to track the cooldown duration

    [Header("Idle Movement Settings")]
    public float idleMoveSpeed = 2f;        // Speed of idle movement
    public float moveDuration = 3f;         // How long the enemy moves in a random direction
    public float idlePauseTime = 2f;        // How long the enemy pauses after moving

    private Transform player;
    private bool playerInSight;
    private EnemyControllerRB theEnemy;

    private Vector3 randomDirection;        // The random direction the enemy moves in
    private float moveTimer;                // Timer to track how long the enemy moves
    private float idleTimer;                // Timer to manage the idle pause
    private bool isMoving = false;          // Flag to track if the enemy is currently moving

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        theEnemy = GetComponent<EnemyControllerRB>();
        animator = GetComponent<Animator>(); // Get the Animator component
        SetRandomDirection(); // Set an initial random direction
    }

    void Update()
    {
        // Handle the attack cooldown timer
        if (isInAttackCooldown)
        {
            attackCooldownTimer += Time.deltaTime;
            if (attackCooldownTimer >= attackCooldown)
            {
                isInAttackCooldown = false;  // Cooldown over, resume movement
                attackCooldownTimer = 0f;
            }
        }

        // Check if the player is within the field of view and distance
        playerInSight = CheckPlayerInSight();

        if (playerInSight)
        {
            Debug.Log("Player detected");
            HandlePlayerChase();
        }
        else
        {
            // Handle random idle movement
            HandleRandomMovement();
        }
    }

    // Handle movement towards the player and attack logic
    private void HandlePlayerChase()
    {
        if (player && !isInAttackCooldown)  // Ensure enemy is not in attack cooldown
        {
            Vector3 playerPos = player.transform.position;
            playerPos.y = 0;  // Keep movement on the same y-plane
            Vector3 enemyPos = transform.position;
            enemyPos.y = 0;

            float distanceToPlayer = Vector3.Distance(enemyPos, playerPos);

            if (distanceToPlayer <= attackRange)
            {
                // If within attack range, set the "Slam" animator bool to true and enter cooldown
                if (animator)
                {
                    animator.SetBool("Smash", true);
                }

                // Stop moving after the attack and start cooldown
                if (theEnemy)
                {
                    theEnemy.Move(Vector3.zero, 0);
                    isInAttackCooldown = true;  // Start attack cooldown
                }
            }
            else
            {
                // If not in attack range, move towards the player
                if (theEnemy)
                {
                    theEnemy.Move((playerPos - enemyPos).normalized, theEnemy.MoveSpeed);
                }
            }
        }
    }

    // Random idle movement logic with a timer
    private void HandleRandomMovement()
    {
        if (isMoving)
        {
            // Move for a set duration
            moveTimer += Time.deltaTime;

            if (moveTimer < moveDuration)
            {
                // Move in the random direction
                if (theEnemy)
                    theEnemy.Move(randomDirection, idleMoveSpeed);
            }
            else
            {
                // Stop moving after the duration
                isMoving = false;
                moveTimer = 0f;
                if (theEnemy)
                    theEnemy.Move(Vector3.zero, 0); // Stop movement
            }
        }
        else
        {
            // Wait for a set idle time before moving again
            idleTimer += Time.deltaTime;

            if (idleTimer >= idlePauseTime)
            {
                SetRandomDirection(); // Pick a new random direction
                isMoving = true;      // Start moving
                idleTimer = 0f;       // Reset idle timer
            }
        }
    }

    // Function to set a random direction for the enemy to move in
    private void SetRandomDirection()
    {
        // Choose a random direction on the x-z plane
        randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }

    // Check if the player is in the field of view and within distance
    bool CheckPlayerInSight()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer < fieldOfViewAngle / 2f)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer < detectionRadius)
            {
                if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstructionLayer))
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Visualize detection radius and FOV in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfViewAngle / 2f, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfViewAngle / 2f, 0) * transform.forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, leftBoundary * detectionRadius);
        Gizmos.DrawRay(transform.position, rightBoundary * detectionRadius);

        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
