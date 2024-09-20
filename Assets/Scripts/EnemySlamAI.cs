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
        SetRandomDirection(); // Set an initial random direction
    }

    void Update()
    {
        // Check if the player is within the field of view and distance
        playerInSight = CheckPlayerInSight();

        if (playerInSight)
        {
            Debug.Log("Player detected");
            if (player)
            {
                Vector3 playerPos = player.transform.position;
                playerPos.y = 0;  // Keep movement on the same y-plane
                Vector3 enemyPos = transform.position;
                enemyPos.y = 0;

                // Move towards the player
                if (theEnemy)
                    theEnemy.Move((playerPos - enemyPos).normalized, theEnemy.MoveSpeed);
            }
        }
        else
        {
            // Handle random idle movement
            HandleRandomMovement();
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
    }
}
