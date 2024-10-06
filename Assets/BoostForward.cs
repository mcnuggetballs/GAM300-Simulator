using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostForward : StateMachineBehaviour
{
    public float dashSpeed = 10f;           // The speed of the dash
    public float dashRange = 5f;            // Maximum range of the overlap sphere
    float sphereRadius = 10.0f;             // Radius of the overlap sphere
    public LayerMask enemyLayer;            // LayerMask to filter enemies
    protected float stopDistance = 0.05f;         // Distance from the enemy at which the player stops dashing
    public float dashDuration = 0.2f;       // Duration of the dash
    public float viewAngleThreshold = 60f;  // Maximum angle difference between player forward and enemy direction

    private Rigidbody rb;
    private float dashStartTime;
    private Vector3 dashDirection;
    private Vector3 targetPosition;         // The target position (enemy's position) to dash towards
    private bool enemyHit;                  // Whether an enemy was hit by the overlap sphere

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Get the Rigidbody component to apply the dash
        rb = animator.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Calculate the dash direction and target position based on the enemy hit by the overlap sphere
            dashDirection = GetDashDirection(animator.transform);

            // Start the dash and record the start time
            dashStartTime = Time.time;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetBool("Hurt"))
            return;
        if (rb != null)
        {
            // If an enemy was hit, stop at the enemy's position
            if (enemyHit && Vector3.Distance(rb.position, targetPosition) <= stopDistance)
            {
                rb.velocity = Vector3.zero;  // Stop the dash
                return;                      // Exit the update
            }

            // If the dash is still in progress (within dashDuration), apply the movement
            if (Time.time < dashStartTime + dashDuration)
            {
                rb.velocity = dashDirection * dashSpeed;
            }
            else
            {
                // Stop the dash by zeroing out the velocity after dash duration
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }
        }
    }

    // Perform an overlap sphere and return the dash direction
    private Vector3 GetDashDirection(Transform playerTransform)
    {
        Collider[] hits = Physics.OverlapSphere(playerTransform.position, sphereRadius, enemyLayer);
        Vector3 direction = playerTransform.forward; // Dash forward by default
        float nearestDistance = float.MaxValue;      // Store the nearest enemy distance

        // Iterate through all hit colliders and find the nearest enemy within the player's view
        foreach (Collider hit in hits)
        {
            Vector3 directionToEnemy = (hit.transform.position - playerTransform.position).normalized;
            float distance = Vector3.Distance(playerTransform.position, hit.transform.position);

            // Check if the enemy is within dash range and within the player's forward view angle
            if (distance < nearestDistance && distance <= dashRange)
            {
                float dotProduct = Vector3.Dot(playerTransform.forward, directionToEnemy);

                // Convert dot product into angle and check if it's within the view angle threshold
                float angleToEnemy = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;
                if (angleToEnemy <= viewAngleThreshold)
                {
                    nearestDistance = distance;
                    targetPosition = hit.transform.position;
                    enemyHit = true;

                    // Calculate the direction towards the nearest enemy
                    direction = (targetPosition - playerTransform.position).normalized;
                }
            }
        }

        // If no enemy is hit or within the player's view, dash forward
        if (!enemyHit)
        {
            return playerTransform.forward;
        }

        return direction;
    }
}
