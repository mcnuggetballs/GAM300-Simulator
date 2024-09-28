using StarterAssets;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class HookSkill : Skill
{
    public float hookRange = 10f;       // Range within which the enemy can be hooked
    public float pullSpeed = 55f;       // Speed at which the enemy is pulled
    public LayerMask targetLayer;
    public LayerMask enemyLayer;
    public LayerMask playerLayer;
    public LineRenderer lineRenderer;   // Reference to the LineRenderer for the hook

    public float hookTravelSpeed = 60f; // Speed at which the hook travels toward the enemy
    public float hookMissDuration = 0.2f; // Duration the hook remains visible when it misses
    public float hitPullDelay = 0.2f;   // Delay before pulling the enemy in after hit
    private float shootDelay = 0.1f;

    // This method will be called to activate the hooking skill
    public override void Activate(GameObject user)
    {
        if (isOnCooldown) return;  // Prevent activation if the skill is on cooldown
        if (user.GetComponent<Animator>())
        {
            user.GetComponent<Animator>().SetBool("Hook",true);
        }
        StartCoroutine(Cast(user));
        
    }
    private IEnumerator Cast(GameObject user)
    {
        AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.MiscSounds[2],0.2f,transform.position);
        yield return new WaitForSeconds(shootDelay);
        if ((enemyLayer.value & (1 << user.layer)) != 0)
        {
            targetLayer = playerLayer;
        }
        else
        {
            targetLayer = enemyLayer;
        }
        // Disable movement during the hook
        if (user.GetComponent<ThirdPersonControllerRB>())
        {
            user.GetComponent<ThirdPersonControllerRB>().disableMovement = true;
        }
        if (user.GetComponent<EnemyControllerRB>())
        {
            user.GetComponent<EnemyControllerRB>().disableMovement = true;
        }
        if ((enemyLayer.value & (1 << user.layer)) != 0)
        {
            // Cast a ray from the camera forward
            Vector3 playerDir = Vector3.zero;
            playerDir = user.transform.forward;
            if (user.GetComponent<EnemyAI>())
            {
                playerDir = user.GetComponent<EnemyAI>().GetCurrentPlayerPos();
                playerDir.y = user.GetComponent<Entity>().neck.position.y;
                playerDir = (playerDir - user.GetComponent<Entity>().neck.position).normalized;
            }
            Ray ray = new Ray(user.GetComponent<Entity>().neck.position, playerDir);
            RaycastHit hit;

            // If the ray hits an enemy
            if (Physics.Raycast(ray, out hit, hookRange, targetLayer))
            {
                if (hit.collider.GetComponent<NavMeshAgent>())
                {
                    hit.collider.GetComponent<NavMeshAgent>().CompleteOffMeshLink();
                    hit.collider.GetComponent<NavMeshAgent>().enabled = false;
                }
                if (hit.collider.GetComponent<PathfindingScript>())
                {
                    hit.collider.GetComponent<PathfindingScript>().isJumping = false;
                    hit.collider.GetComponent<PathfindingScript>().enabled = false;
                }
                // Enable the LineRenderer before launching the hook
                lineRenderer.enabled = true;
                AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.MiscSounds[3],0.2f,transform.position);
                // Start launching the hook toward the enemy
                StartCoroutine(LaunchHook(hit.collider.gameObject, user));
            }
            else
            {
                // No enemy hit, show the hook going to the point where the raycast hit the environment
                Vector3 hookMissPosition = ray.origin + ray.direction * hookRange;

                // Show the miss with LineRenderer
                StartCoroutine(ShowMiss(user, hookMissPosition));
            }
        }
        else
        {
            // Cast a ray from the camera forward
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            // If the ray hits an enemy
            if (Physics.Raycast(ray, out hit, hookRange, targetLayer))
            {
                if (hit.collider.GetComponent<NavMeshAgent>())
                {
                    hit.collider.GetComponent<NavMeshAgent>().CompleteOffMeshLink();
                    hit.collider.GetComponent<NavMeshAgent>().enabled = false;
                }
                if (hit.collider.GetComponent<PathfindingScript>())
                {
                    hit.collider.GetComponent<PathfindingScript>().isJumping = false;
                    hit.collider.GetComponent<PathfindingScript>().enabled = false;
                }
                // Enable the LineRenderer before launching the hook
                lineRenderer.enabled = true;
                AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.MiscSounds[3],0.2f,transform.position);

                // Start launching the hook toward the enemy
                StartCoroutine(LaunchHook(hit.collider.gameObject, user));
            }
            else
            {
                // No enemy hit, show the hook going to the point where the raycast hit the environment
                Vector3 hookMissPosition = ray.origin + ray.direction * hookRange;

                // Show the miss with LineRenderer
                StartCoroutine(ShowMiss(user, hookMissPosition));
            }
        }

        // Start the skill cooldown
        StartCoroutine(Cooldown());
    }

    private IEnumerator LaunchHook(GameObject enemy, GameObject user)
    {
        Transform hookStartPoint = null;
        // Set initial LineRenderer positions
        if (user.GetComponent<Entity>())
        {
            hookStartPoint = user.GetComponent<Entity>().leftHand;
        }
        else
        {
            hookStartPoint = user.transform;
        }
        lineRenderer.SetPosition(0, hookStartPoint.position);  // Start point from public Transform
        lineRenderer.SetPosition(1, hookStartPoint.position);  // End point (initially same as start)

        Vector3 targetPosition = Vector3.zero;
        if (enemy.GetComponent<Entity>())
        {
            targetPosition = enemy.GetComponent<Entity>().neck.position;
        }
        else
        {
            targetPosition = enemy.transform.position;
        }
        float distanceToEnemy = Vector3.Distance(hookStartPoint.position, targetPosition);

        // Animate the line moving towards the enemy
        float traveledDistance = 0f;

        while (traveledDistance < distanceToEnemy)
        {
            traveledDistance += hookTravelSpeed * Time.deltaTime;
            Vector3 hookPosition = Vector3.Lerp(hookStartPoint.position, targetPosition, traveledDistance / distanceToEnemy);

            // Update LineRenderer to show hook moving toward the enemy
            lineRenderer.SetPosition(0, hookStartPoint.position);  // Always set the first position to the hook start point's position
            lineRenderer.SetPosition(1, hookPosition);             // Update the second position to hook's current position

            yield return null;  // Wait until the next frame
        }

        // Hook reaches the enemy, delay for 0.2 seconds before pulling
        yield return new WaitForSeconds(hitPullDelay);

        // Start pulling the enemy toward the player
        StartCoroutine(PullEnemy(enemy, user));
    }

    private IEnumerator PullEnemy(GameObject enemy, GameObject user)
    {
        Transform hookStartPoint = null;
        // Set initial LineRenderer positions
        if (user.GetComponent<Entity>())
        {
            hookStartPoint = user.GetComponent<Entity>().leftHand;
        }
        else
        {
            hookStartPoint = user.transform;
        }
        // Pull the enemy toward the player while updating the LineRenderer
        while (Vector3.Distance(enemy.transform.position, user.transform.position) > 1f)
        {
            Vector3 direction = (user.transform.position - enemy.transform.position).normalized;
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, user.transform.position, pullSpeed * Time.deltaTime);

            // Update LineRenderer positions to show the pulling back effect
            if (hookStartPoint)
                lineRenderer.SetPosition(0, hookStartPoint.position);  // Set start point to the hook start point's current position
            else
                lineRenderer.SetPosition(0, user.transform.position);  // Set start point to the hook start point's current position

            if (enemy.GetComponent<Entity>())
            {
                lineRenderer.SetPosition(1, enemy.GetComponent<Entity>().neck.position); // Set end point to the enemy's current position
            }
            else
            {
                lineRenderer.SetPosition(1, enemy.transform.position); // Set end point to the enemy's current position
            }

            yield return null;  // Wait until the next frame
        }

        // Re-enable movement for the user after pulling is done
        if (user.GetComponent<EnemyControllerRB>())
        {
            user.GetComponent<EnemyControllerRB>().disableMovement = false;
        }
        if (user.GetComponent<ThirdPersonControllerRB>())
        {
            user.GetComponent<ThirdPersonControllerRB>().disableMovement = false;
        }

        // Disable the LineRenderer once the hook is complete
        lineRenderer.enabled = false;
        if (enemy.GetComponent<NavMeshAgent>())
        {
            enemy.GetComponent<NavMeshAgent>().enabled = true;
        }

        if (enemy.GetComponent<PathfindingScript>())
        {
            enemy.GetComponent<PathfindingScript>().enabled = true;
        }
    }

    private IEnumerator ShowMiss(GameObject user, Vector3 missPosition)
    {
        Transform hookStartPoint = null;
        // Set initial LineRenderer positions
        if (user.GetComponent<Entity>())
        {
            hookStartPoint = user.GetComponent<Entity>().leftHand;
        }
        else
        {
            hookStartPoint = user.transform;
        }

        // Enable the LineRenderer
        lineRenderer.enabled = true;

        // Set the initial position of the LineRenderer at the hook start point
        lineRenderer.SetPosition(0, hookStartPoint.position);  // Start at the hook start point
        lineRenderer.SetPosition(1, hookStartPoint.position);  // Initially, the end is also at the start

        // The total distance the hook should travel (to the miss position)
        float totalDistance = Vector3.Distance(hookStartPoint.position, missPosition);
        float traveledDistance = 0f;

        // Animate the line towards the miss position
        while (traveledDistance < totalDistance)
        {
            // Increment the traveled distance based on hook travel speed
            traveledDistance += hookTravelSpeed * Time.deltaTime;

            // Calculate the new hook position based on the traveled distance
            Vector3 hookPosition = Vector3.Lerp(hookStartPoint.position, missPosition, traveledDistance / totalDistance);

            // Update the LineRenderer to extend toward the miss position
            lineRenderer.SetPosition(1, hookPosition);  // Set the second position to the current hook position

            yield return null;  // Wait until the next frame
        }

        // Once the hook reaches its maximum distance, wait for a short duration to show the miss
        yield return new WaitForSeconds(hookMissDuration);

        // Disable the LineRenderer after the miss is shown
        lineRenderer.enabled = false;

        // Re-enable movement after the miss
        if (user.GetComponent<EnemyControllerRB>())
        {
            user.GetComponent<EnemyControllerRB>().disableMovement = false;
        }
        if (user.GetComponent<ThirdPersonControllerRB>())
        {
            user.GetComponent<ThirdPersonControllerRB>().disableMovement = false;
        }
    }

}
