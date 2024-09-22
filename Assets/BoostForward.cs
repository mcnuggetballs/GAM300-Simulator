using StarterAssets;
using UnityEngine;

public class BoostForward : StateMachineBehaviour
{
    [SerializeField] private float dashSpeed = 5.0f; // Speed of the dash
    [SerializeField] private float dashDuration = 0.2f; // Duration of the dash in seconds
    private ThirdPersonControllerRB controller; // Reference to the character controller
    private float elapsedTime = 0.0f; // Timer to track the duration of the dash

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Get reference to the character controller
        if (controller == null)
        {
            controller = animator.GetComponent<ThirdPersonControllerRB>();
        }

        // Temporarily disable normal movement control
        if (controller != null)
        {
            controller.disableMovement = true;
            elapsedTime = 0.0f; // Reset the timer
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (controller != null && elapsedTime < dashDuration)
        {
            // Calculate the forward direction based on the character's facing direction
            Vector3 forwardDirection = controller.transform.forward;

            // Apply the dash movement by setting the Rigidbody's velocity
            controller.GetComponent<Rigidbody>().velocity = forwardDirection * dashSpeed;

            // Update the elapsed time
            elapsedTime += Time.deltaTime;
        }
        else if (controller != null)
        {
            // Stop the dash by zeroing out forward velocity once the dash is complete
            controller.GetComponent<Rigidbody>().velocity = new Vector3(0, controller.GetComponent<Rigidbody>().velocity.y, 0);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Re-enable normal movement control
        if (controller != null)
        {
            controller.disableMovement = false;
        }
    }
}
