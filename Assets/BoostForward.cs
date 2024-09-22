using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostForward : StateMachineBehaviour
{
    public float dashSpeed = 10f;           // The speed of the dash
    public float dashRange = 5f;            // Maximum range of the sphere cast
    public float sphereCastRadius = 0.5f;   // Radius of the sphere cast
    public LayerMask enemyLayer;            // LayerMask to filter enemies
    public float stopDistance = 1f;         // Distance from the enemy at which the player stops dashing
    public float dashDuration = 0.2f;       // Duration of the dash

    private Rigidbody rb;
    private float dashStartTime;
    private Vector3 dashDirection;
    private Vector3 targetPosition;         // The target position (enemy's position) to dash towards
    private bool enemyHit;                  // Whether an enemy was hit by the sphere cast

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if ((enemyLayer.value & (1 << animator.gameObject.layer)) != 0)
        {
            enemyLayer = LayerMask.GetMask("Player");
        }
        else
        {
            enemyLayer = LayerMask.GetMask("Enemy");
        }

        // Get the Rigidbody component to apply the dash
        rb = animator.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Calculate the dash direction and target position based on the enemy hit by the sphere cast
            dashDirection = GetDashDirection(animator.transform);

            // Start the dash and record the start time
            dashStartTime = Time.time;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
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

    // Perform a sphere cast and return the dash direction
    private Vector3 GetDashDirection(Transform playerTransform)
    {
        RaycastHit hit;
        Vector3 origin = playerTransform.position; // Start slightly above the ground
        Vector3 direction = playerTransform.forward; // Dash forward by default

        // Perform a sphere cast in the forward direction
        if (Physics.SphereCast(origin, sphereCastRadius, direction, out hit, dashRange, enemyLayer))
        {
            Vector3 hitPoint = hit.point;
            hitPoint.y = playerTransform.position.y;
            // If an enemy is hit, store the target position
            targetPosition = hitPoint;
            enemyHit = true;

            // Return the direction towards the hit point (enemy's position)
            return (hitPoint - playerTransform.position).normalized;
        }

        // If no enemy is hit, return the player's forward direction
        enemyHit = false;
        return direction;
    }

}
