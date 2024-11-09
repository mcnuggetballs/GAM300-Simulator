using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ShootSkill : Skill
{
    [SerializeField] private GameObject projectilePrefab;
    private float projectileDelay = 1.3f; // Delay before firing the projectile
    public LayerMask targetLayer;
    public LayerMask enemyLayer;
    public LayerMask playerLayer;
    private LineRenderer lineRenderer;
    bool isShooting = false;
    GameObject user = null;
    Coroutine shootingCoroutine;
    Vector3 facingDir = Vector3.zero;
    bool facing = false;
    private void Awake()
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.GetComponent<LineRenderer>();
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
            lineRenderer.enabled = false; // Hide initially
        }
    }
    public override bool Activate(GameObject user)
    {
        if (isOnCooldown)
            return false;
        if ((playerLayer.value & (1 << user.layer)) != 0)
        {
            user.GetComponent<Animator>().SetBool("IgnoreStun", true);
            AudioManager.instance.PlayCachedSound(AudioManager.instance.MCShootSkillBarks, user.transform.position, 1.0f);

            Camera camera = Camera.main;
            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
            facingDir = ray.direction;
            user.GetComponent<ThirdPersonControllerRB>().RotateTo(facingDir);
            facing = true;
        }
        shootingCoroutine = StartCoroutine(ShootProjectile(user));
        StartCoroutine(Cooldown());

        return true;
    }
    private void Update()
    {
        if (isShooting && user)
        {
            Vector3 startPoint, targetPoint;
            if (user != null)
            {
                startPoint = user.GetComponent<Entity>().leftHand.position;
                if (user.GetComponent<EnemyAI>())
                {
                    targetPoint = user.GetComponent<EnemyAI>().GetCurrentPlayerNeckPos();
                    ShowLine(startPoint, targetPoint);
                }
            }
        }
        if (facing == true)
        {
            user.GetComponent<ThirdPersonControllerRB>().RotateTo(facingDir);
        }

    }
    public void DisableEverything()
    {
        if (shootingCoroutine != null)
            StopCoroutine(shootingCoroutine);
        shootingCoroutine = null;
        if (user && user.GetComponent<Animator>())
            user.GetComponent<Animator>().SetBool("Hook", false);
        this.user = null;
        isShooting = false;
        lineRenderer.enabled = false;
    }
    private IEnumerator ShootProjectile(GameObject user)
    {
        if ((enemyLayer.value & (1 << user.layer)) != 0)
        {
            this.user = user;
            isShooting = true;
            lineRenderer.enabled = true;
            AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.EnemyShooterSounds[5], user.transform.position);
            GameObject aim = Instantiate(Resources.Load("SHOOTERAIM", typeof(GameObject)) as GameObject, user.GetComponent<EnemyAI>().GetCurrentPlayerNeckPos(), Quaternion.identity,user.GetComponent<EnemyAI>().GetCurrentPlayerTransform());
            if (user.GetComponent<Animator>())
                user.GetComponent<Animator>().SetBool("Hook", true);
            aim.transform.position -= Camera.main.transform.forward * 0.3f;
            yield return new WaitForSeconds(projectileDelay);
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            if (user.GetComponent<Animator>())
                user.GetComponent<Animator>().SetBool("Hook", true);
            yield return new WaitForSeconds(0.2f);
        }
        AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.EnemyShooterSounds[1], transform.position);
        // Case for when user is an enemy
        if ((enemyLayer.value & (1 << user.layer)) != 0)
        {
            EnemyAI shooterAI = user.GetComponent<EnemyAI>();
            if (shooterAI != null && projectilePrefab != null)
            {
                AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.EnemyShooterSounds[4], user.transform.position);
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

                AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.EnemyShooterSounds[4], user.transform.position);
                GameObject projectile = Instantiate(projectilePrefab, entity.leftHand.position, Quaternion.identity);
                Projectile projectileScript = projectile.GetComponent<Projectile>();
                if (projectileScript != null)
                {
                    projectileScript.Initialize(shootDirection, user);
                }
                user.GetComponent<Animator>().SetBool("IgnoreStun", false);
            }
        }

        if (user.GetComponent<Animator>())
            user.GetComponent<Animator>().SetBool("Hook", false);
        this.user = null;
        isShooting = false;
        lineRenderer.enabled = false;
        facing = false;
    }
    private void ShowLine(Vector3 startPoint, Vector3 endPoint)
    {
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }
}
