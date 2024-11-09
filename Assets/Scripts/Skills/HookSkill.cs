using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;

public class HookSkill : Skill
{
    public GameObject hookProjectilePrefab; // Prefab for the hook projectile
    protected float hookRange = 16.0f;           // Maximum range of the hook
    public float pullSpeed = 20f;           // Speed at which the player is pulled
    float projectileSpeed = 13.0f;     // Speed of the projectile
    public LayerMask targetLayer;           // Layer for detecting player
    public LayerMask enemyLayer;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;

    private GameObject projectile;          // Reference to the instantiated projectile
    private Vector3 targetPosition;         // The position to which the projectile travels
    private bool returning;                 // Flag to check if the projectile is returning
    float playerStunDuration = 0.3f;

    public float chainLinkLength = 0.5f;
    public GameObject chainLinkPrefab;
    int numChainLinks = 50;
    private List<GameObject> chainLinks = new List<GameObject>();
    GameObject userTest = null;
    bool hitPlayer = false;

    // Activate method to trigger the skill
    public override bool Activate(GameObject user)
    {
        userTest = user;
        // Check if the skill is on cooldown
        if (isOnCooldown) return false;

        user.GetComponent<Animator>().SetBool("Hook", true);
        if ((enemyLayer.value & (1 << user.layer)) != 0)
        {
            hitPlayer = false;
            targetLayer = playerLayer;
            StartCoroutine(HookProjectileRoutine(user));
            projectileSpeed = 13.0f;
            pullSpeed = 20.0f;
        }
        else
        {
            if (user.GetComponent<ThirdPersonControllerRB>())
            {
                user.GetComponent<ThirdPersonControllerRB>().disableMovement = true;
                user.GetComponent<ThirdPersonControllerRB>().StopMovement();
                projectileSpeed = 25.0f;
                pullSpeed = 25.0f;
            }
            targetLayer = enemyLayer;
            StartCoroutine(HookProjectileRoutinePlayer(user));
            user.GetComponent<Animator>().SetBool("IgnoreStun", true);
        }
        // Start the hook projectile coroutine
        // Start cooldown
        StartCoroutine(Cooldown());
        return true;
    }
    private IEnumerator HookProjectileRoutinePlayer(GameObject user)
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Center of the screen
        RaycastHit hit;

        float sphereCastRadius = 5.0f; // Adjust the radius of the sphere as needed
        Transform cameraTransform = Camera.main.transform;
        Vector3 aimDirection = cameraTransform.forward;
        // Perform a SphereCast along the ray, checking for collisions with targetLayer and obstacleLayer
        if (Physics.SphereCast(ray, sphereCastRadius, out hit, hookRange, targetLayer))
        {
            // If the SphereCast hits something within the range, set the target position to the hit point
            targetPosition = hit.point;
            aimDirection = targetPosition - user.GetComponent<Entity>().leftHand.position;
            aimDirection.Normalize();
        }
        else
        {
            // If no hit, set the target position to max range in front of the user
            targetPosition = user.GetComponent<Entity>().leftHand.position + aimDirection * hookRange;
        }
        user.GetComponent<ThirdPersonControllerRB>().RotateTo(aimDirection);
        //GetComponent<ThirdPersonControllerRB>().RotateTo(aimDirection);
        // Spawn the projectile at the user's position
        projectile = Instantiate(Resources.Load("HookProjectile") as GameObject, user.GetComponent<Entity>().leftHand.position, Quaternion.LookRotation(aimDirection));
        returning = false;

        // Calculate number of links based on the distance to target
        float distanceToTarget = Vector3.Distance(user.GetComponent<Entity>().leftHand.position, targetPosition);
        int calculatedLinks = Mathf.FloorToInt(distanceToTarget / chainLinkLength);
        
        // Clear any existing links if they exist
        foreach (GameObject link in chainLinks)
            Destroy(link);
        chainLinks.Clear();

        // Create the exact number of chain links needed
        for (int i = 0; i < calculatedLinks; i++)
        {
            GameObject newLink = Instantiate(chainLinkPrefab, user.GetComponent<Entity>().leftHand.position, Quaternion.identity);
            chainLinks.Add(newLink);
        }

        // Move the projectile toward the target or until it hits the player or obstacle
        while (Vector3.Distance(projectile.transform.position, targetPosition) > 0.1f && !returning)
        {
            projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, targetPosition, projectileSpeed * Time.deltaTime);
            UpdateChainLinkPositions(user);

            // Check for collision with the player
            if (Physics.CheckSphere(projectile.transform.position, 0.5f, targetLayer, QueryTriggerInteraction.Ignore))
            {
                Collider[] hits = Physics.OverlapSphere(projectile.transform.position, 0.5f, targetLayer);
                if (hits.Length > 0)
                {
                    GameObject player = hits[0].gameObject;
                    player.GetComponent<Animator>().SetTrigger("Stun");
                    player.GetComponent<Entity>().TakeDamage(1, Vector3.zero, 0, true);
                    yield return new WaitForSeconds(playerStunDuration);
                    StartCoroutine(PullPlayer(player, user));
                    hitPlayer = true;
                    returning = true;
                    user.GetComponent<Animator>().SetBool("Hook", false);
                }
            }
            else if (Physics.CheckSphere(projectile.transform.position, 0.5f, obstacleLayer, QueryTriggerInteraction.Ignore))
            {
                returning = true;
                user.GetComponent<Animator>().SetBool("Hook", false);
            }

            yield return null;
        }

        // Start returning the projectile if it reaches max range or hits an object
        returning = true;
        user.GetComponent<Animator>().SetBool("Hook", false);
        while (returning && Vector3.Distance(projectile.transform.position, user.GetComponent<Entity>().leftHand.position) > 0.1f)
        {
            projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, user.GetComponent<Entity>().leftHand.position, pullSpeed * Time.deltaTime);
            UpdateChainLinkPositions(user);
            yield return null;
        }

        // Destroy projectile and chain links
        Destroy(projectile);
        foreach (GameObject link in chainLinks)
            Destroy(link);
        chainLinks.Clear();
        if (user.GetComponent<ThirdPersonControllerRB>())
            user.GetComponent<ThirdPersonControllerRB>().disableMovement = false;
        user.GetComponent<Animator>().SetBool("IgnoreStun", false);
    }
    private IEnumerator HookProjectileRoutine(GameObject user)
    {
        Transform playerTransform = user.GetComponent<HookEnemyAI>().GetCurrentPlayerTransform();
        Vector3 toPlayerDir = (playerTransform.GetComponent<Entity>().neck.position - user.GetComponent<Entity>().leftHand.position).normalized;
        // Spawn the projectile at the user's position
        projectile = Instantiate(Resources.Load("HookProjectile") as GameObject, user.GetComponent<Entity>().leftHand.position, Quaternion.LookRotation(toPlayerDir));
        returning = false;

        // Calculate the direction and target position based on hook range
        targetPosition = user.GetComponent<Entity>().leftHand.position + toPlayerDir * hookRange;

        // Calculate number of links based on the distance to target
        float distanceToTarget = Vector3.Distance(user.GetComponent<Entity>().leftHand.position, targetPosition);
        int calculatedLinks = Mathf.FloorToInt(distanceToTarget / chainLinkLength);

        // Clear any existing links if they exist
        foreach (GameObject link in chainLinks)
            Destroy(link);
        chainLinks.Clear();

        // Create the exact number of chain links needed
        for (int i = 0; i < calculatedLinks; i++)
        {
            GameObject newLink = Instantiate(chainLinkPrefab, user.GetComponent<Entity>().leftHand.position, Quaternion.identity);
            chainLinks.Add(newLink);
        }

        float timer = 0.0f;
        float homeTime = 0.3f;
        // Move the projectile toward the target or until it hits the player
        while (Vector3.Distance(projectile.transform.position, targetPosition) > 0.1f && !returning)
        {
            timer += Time.deltaTime;
            if (timer < homeTime)
            {
                toPlayerDir = (playerTransform.GetComponent<Entity>().neck.position - user.GetComponent<Entity>().leftHand.position).normalized;
                targetPosition = user.GetComponent<Entity>().leftHand.position + toPlayerDir * hookRange;
            }

            projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, targetPosition, projectileSpeed * Time.deltaTime);
            UpdateChainLinkPositions(user);
            // Check for collision with the player
            if (Physics.CheckSphere(projectile.transform.position, 0.5f, targetLayer, QueryTriggerInteraction.Ignore))
            {
                // Pull the player towards the user
                Collider[] hits = Physics.OverlapSphere(projectile.transform.position, 0.5f, targetLayer);
                if (hits.Length > 0)
                {
                    GameObject player = hits[0].gameObject;
                    player.GetComponent<Animator>().SetTrigger("Stun");
                    player.GetComponent<Entity>().TakeDamage(1, Vector3.zero, 0);
                    yield return new WaitForSeconds(playerStunDuration);
                    StartCoroutine(PullPlayer(player, user));
                    hitPlayer = true;
                    returning = true; // Trigger return after hitting the player
                    user.GetComponent<Animator>().SetBool("Hook", false);
                }
            }
            else if (Physics.CheckSphere(projectile.transform.position, 0.5f, obstacleLayer, QueryTriggerInteraction.Ignore))
            {
                returning = true;

                user.GetComponent<Animator>().SetBool("Hook", false);
            }

            yield return null;
        }

        // After reaching max range or hitting the player, start returning to sender
        returning = true;
        user.GetComponent<Animator>().SetBool("Hook", false);
        while (returning && Vector3.Distance(projectile.transform.position, user.GetComponent<Entity>().leftHand.position) > 0.1f)
        {
            projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, user.GetComponent<Entity>().leftHand.position, pullSpeed * Time.deltaTime);
            UpdateChainLinkPositions(user);
            yield return null;
        }

        // Destroy the projectile after return
        Destroy(projectile);
        foreach (GameObject link in chainLinks)
            Destroy(link);
        chainLinks.Clear();
        if (!hitPlayer)
        {
            user.GetComponent<HookEnemyAI>().hasPulled = false;
            user.GetComponent<HookEnemyAI>().switchState = true;
            user.GetComponent<HookEnemyAI>().complete = true;
        }
        else if (hitPlayer)
        {
            user.GetComponent<HookEnemyAI>().hasPulled = true;
        }
    }

    private void UpdateChainLinkPositions(GameObject user)
    {
        // Recalculate distance and number of links based on the current projectile position
        float currentDistance = Vector3.Distance(user.GetComponent<Entity>().leftHand.position, projectile.transform.position);
        int requiredLinks = Mathf.FloorToInt(currentDistance / chainLinkLength);

        // Add or remove links if needed
        if (requiredLinks != chainLinks.Count)
        {
            foreach (GameObject link in chainLinks)
                Destroy(link);
            chainLinks.Clear();

            for (int i = 0; i < requiredLinks; i++)
            {
                GameObject newLink = Instantiate(chainLinkPrefab, user.GetComponent<Entity>().leftHand.position, Quaternion.identity);
                chainLinks.Add(newLink);
            }
        }

        // Position each chain link between the user and the projectile
        for (int i = 0; i < chainLinks.Count; i++)
        {
            float t = (float)(i + 1) / (chainLinks.Count + 1);
            chainLinks[i].transform.position = Vector3.Lerp(user.GetComponent<Entity>().leftHand.position, projectile.transform.position, t);
            chainLinks[i].transform.rotation = Quaternion.LookRotation(projectile.transform.position - chainLinks[i].transform.position);
        }
    }

    private IEnumerator PullPlayer(GameObject player, GameObject user)
    {
        // Pull the player towards the user until close enough
        while (Vector3.Distance(player.transform.position, user.transform.position) > 1f)
        {
            if (projectile != null)
                projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, user.GetComponent<Entity>().leftHand.position, pullSpeed * Time.deltaTime);
            Vector3 direction = (user.transform.position - player.transform.position).normalized;
            player.transform.position = Vector3.MoveTowards(player.transform.position, user.transform.position, pullSpeed * Time.deltaTime);
            yield return null;
        }
    }
    private void Update()
    {
        if (userTest != null && projectile != null)
            UpdateChainLinkPositions(userTest);
    }
}
