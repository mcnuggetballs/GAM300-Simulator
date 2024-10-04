using System.Collections;
using UnityEngine;

public class ShootSkill : Skill
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileDelay = 0.2f; // Delay before firing the projectile
    public LayerMask targetLayer;
    public LayerMask enemyLayer;
    public LayerMask playerLayer;

    public override bool Activate(GameObject user)
    {
        if (isOnCooldown)
            return false;

        if (user.GetComponent<Animator>())
            user.GetComponent<Animator>().SetTrigger("Hook");
        StartCoroutine(ShootProjectile(user));
        StartCoroutine(Cooldown());

        return true;
    }

    private IEnumerator ShootProjectile(GameObject user)
    {
        yield return new WaitForSeconds(projectileDelay);
        AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.EnemyShooterSounds[1], transform.position);
        // Case for when user is an enemy
        if ((enemyLayer.value & (1 << user.layer)) != 0)
        {
            EnemyAI shooterAI = user.GetComponent<EnemyAI>();
            if (shooterAI != null && projectilePrefab != null)
            {
                GameObject projectile = Instantiate(projectilePrefab, shooterAI.GetComponent<Entity>().leftHand.position, Quaternion.identity);
                Vector3 directionToPlayer = (shooterAI.GetCurrentPlayerNeckPos() - shooterAI.GetComponent<Entity>().leftHand.position).normalized;

                Projectile projectileScript = projectile.GetComponent<Projectile>();
                if (projectileScript != null)
                {
                    projectileScript.Initialize(directionToPlayer, user);
                }
            }
        }
        else
        {
            Entity entity = user.GetComponent<Entity>();
            if (entity != null && projectilePrefab != null)
            {
                Camera camera = Camera.main;

                Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
                RaycastHit hit;
                float sphereRadius = 5.0f;

                Vector3 targetPoint;
                if (Physics.SphereCast(ray, sphereRadius, out hit, Mathf.Infinity, targetLayer))
                {
                    targetPoint = hit.point;
                }
                else
                {
                    targetPoint = ray.GetPoint(1000f);
                }

                Vector3 shootDirection = (targetPoint - entity.leftHand.position).normalized;

                GameObject projectile = Instantiate(projectilePrefab, entity.leftHand.position, Quaternion.identity);
                Projectile projectileScript = projectile.GetComponent<Projectile>();
                if (projectileScript != null)
                {
                    projectileScript.Initialize(shootDirection, user);
                }
            }
        }
    }
}
