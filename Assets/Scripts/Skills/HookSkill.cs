using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HookSkill : Skill
{
    public GameObject hookProjectilePrefab; // Prefab for the hook projectile
    protected float hookRange = 5.0f;           // Maximum range of the hook
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
    public GameObject chainLinkPrefab;
    public int numChainLinks = 20;
    private List<GameObject> chainLinks = new List<GameObject>();

    // Activate method to trigger the skill
    public override bool Activate(GameObject user)
    {
        // Check if the skill is on cooldown
        if (isOnCooldown) return false;

        if ((enemyLayer.value & (1 << user.layer)) != 0)
        {
            targetLayer = playerLayer;
        }
        else
        {
            targetLayer = enemyLayer;
        }
        // Start the hook projectile coroutine
        StartCoroutine(HookProjectileRoutine(user));

        // Start cooldown
        StartCoroutine(Cooldown());
        return true;
    }

    private IEnumerator HookProjectileRoutine(GameObject user)
    {
        // Spawn the projectile at the user's position
        projectile = Instantiate(Resources.Load("HookProjectile") as GameObject, user.GetComponent<Entity>().neck.position, user.transform.rotation);
        returning = false;

        // Calculate the direction and target position based on hook range
        targetPosition = user.GetComponent<Entity>().neck.position + user.transform.forward * hookRange;

        for (int i = 0; i < numChainLinks; i++)
        {
            GameObject newLink = Instantiate(chainLinkPrefab, user.GetComponent<Entity>().neck.position, user.transform.rotation);
            chainLinks.Add(newLink);
        }

        // Move the projectile toward the target or until it hits the player
        while (Vector3.Distance(projectile.transform.position, targetPosition) > 0.1f && !returning)
        {
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
                    yield return new WaitForSeconds(playerStunDuration);
                    StartCoroutine(PullPlayer(player, user));
                    returning = true; // Trigger return after hitting the player
                }
            }
            else if (Physics.CheckSphere(projectile.transform.position, 0.5f, obstacleLayer, QueryTriggerInteraction.Ignore))
            {
                returning = true;
            }

            yield return null;
        }

        // After reaching max range or hitting the player, start returning to sender
        returning = true;
        while (returning && Vector3.Distance(projectile.transform.position, user.GetComponent<Entity>().neck.position) > 0.1f)
        {
            projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, user.GetComponent<Entity>().neck.position, pullSpeed * Time.deltaTime);
            UpdateChainLinkPositions(user);
            yield return null;
        }

        // Destroy the projectile after return
        Destroy(projectile);
        foreach (GameObject link in chainLinks)
            Destroy(link);
        chainLinks.Clear();
        user.GetComponent<HookEnemyAI>().switchState = true;
        user.GetComponent<HookEnemyAI>().complete = true;
    }
    private void UpdateChainLinkPositions(GameObject user)
    {
        // Calculate distance between each link
        float segmentLength = Vector3.Distance(user.GetComponent<Entity>().neck.position, projectile.transform.position) / (numChainLinks + 1);

        // Position each chain link along the line between user and projectile
        for (int i = 0; i < numChainLinks; i++)
        {
            float t = (float)(i + 1) / (numChainLinks + 1);  // Get normalized position between user and projectile
            chainLinks[i].transform.position = Vector3.Lerp(user.GetComponent<Entity>().neck.position, projectile.transform.position, t);
        }
    }

    private IEnumerator PullPlayer(GameObject player, GameObject user)
    {
        // Pull the player towards the user until close enough
        while (Vector3.Distance(player.transform.position, user.transform.position) > 1f)
        {
            projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, user.GetComponent<Entity>().neck.position, pullSpeed * Time.deltaTime);
            Vector3 direction = (user.transform.position - player.transform.position).normalized;
            player.transform.position = Vector3.MoveTowards(player.transform.position, user.transform.position, pullSpeed * Time.deltaTime);
            yield return null;
        }
        user.GetComponent<HookEnemyAI>().hasPulled = true;
    }
}
