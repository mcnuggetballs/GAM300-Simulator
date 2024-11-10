using StarterAssets;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
[Serializable]
public class EntityDeathEvent : UnityEvent { };
public class Entity : MonoBehaviour
{
    [SerializeField] protected float maxHealth = 100;
    [SerializeField] protected float currentHealth;
    [SerializeField] protected float baseDamage = 10;
    Animator animator;
    public Skill skill;
    public GameObject skillPrefab;

    bool hasBeenHit = false;
    Vector3 hitDir;
    float knockBackAmount = 0;
    float hitDuration = 0.1f;
    float hitTimer = 0;

    [Header("Body")]
    public Transform rightHand;
    public Transform leftHand;
    public Transform neck;

    private Rigidbody _rigidbody;
    private Coroutine knockbackCoroutine;

    [SerializeField]
    public GameObject experienceOrbPrefab;
    public int numOrbs = 5;

    [SerializeField]
    public GameObject[] nutsAndBolts;
    public int amountExploded;

    [SerializeField]
    public GameObject coffeePrefab;
    public int coffeeAmount = 0;
    public float coffeeDropChance = 33.3333f;

    [SerializeField]
    public float iFrameDuration = 1.0f;
    protected float iFrameTimer = 0.0f;
    protected bool canBeHit = true;

    [SerializeField]
    public EntityDeathEvent deathEvent;
    [SerializeField]
    public float deathEventDelay = 0.0f;
    public GameObject deathDrop;
    public EntityDeathEvent deathDropPickupEvent;
    public void SetDeathDelayDuration(float delay)
    {
        deathEventDelay = delay;
    }
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetHealthFraction()
    {
        return currentHealth / maxHealth;
    }

    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        if (skillPrefab)
            skill = Instantiate(skillPrefab).GetComponent<Skill>();
    }

    public void TakeDamage(float value)
    {
        currentHealth -= value;
        if (animator != null)
        {
            if (canBeHit)
            {
                animator.SetTrigger("Hurt");
                canBeHit = false;
            }
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(float value, Vector3 hitDirection, float kb, bool ignoreIFrame = false)
    {
        if (GetComponent<EnemyAI>())
        {
            AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.EnemyHurtSounds[UnityEngine.Random.Range(0, AudioManager.instance.EnemyHurtSounds.Length)], transform.position);
            ExplodeNutsAndBolts();
        }
        if (GetComponent<PlayerHack>())
        {
            AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.PlayerHurtSounds[UnityEngine.Random.Range(0, AudioManager.instance.PlayerHurtSounds.Length)], animator.transform.position);
            IngameUIManager.Instance.TriggerHurtOverlay();
            IngameUIManager.Instance.GetComponent<Animator>().SetTrigger("HitFlash");
        }
        hitDir = hitDirection;
        knockBackAmount = kb;
        currentHealth -= value;
        hasBeenHit = true;

        // Trigger hurt animation
        if (animator != null)
        {
            if (canBeHit)
            {
                animator.SetTrigger("Hurt");
                StartCoroutine(ResetTriggerAfterDelay("Hurt", 0.1f));  // Adjust delay as needed
                if (!ignoreIFrame)
                    canBeHit = false;
            }
        }

        // Handle knockback if the enemy is still alive
        if (currentHealth > 0)
        {
            if (knockbackCoroutine != null)
            {
                StopCoroutine(knockbackCoroutine);
            }
            knockbackCoroutine = StartCoroutine(ApplyKnockback(hitDir, knockBackAmount, hitDuration));
        }
        else
        {
            Die();
        }
    }

    private IEnumerator ApplyKnockback(Vector3 direction, float knockbackForce, float duration)
    {
        float timer = 0;
        Vector3 knockbackVelocity = direction * knockbackForce;

        //// Temporarily stop movement or AI control here if needed
        //if (GetComponent<EnemyControllerRB>())
        //{
        //    GetComponent<EnemyControllerRB>().disableMovement = true;
        //}

        while (timer < duration)
        {
            timer += Time.deltaTime;

            // Smoothly apply knockback over time
            _rigidbody.velocity = new Vector3(knockbackVelocity.x, _rigidbody.velocity.y, knockbackVelocity.z);

            yield return null; // Wait until the next frame
        }

        //// Re-enable movement after knockback is done
        //if (GetComponent<EnemyControllerRB>())
        //{
        //    GetComponent<EnemyControllerRB>().disableMovement = false;
        //}

        // Reset velocity after knockback
        _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
    }
    private IEnumerator DeathEventDelay()
    {
        yield return new WaitForSeconds(deathEventDelay);
        deathEvent.Invoke();
    }
    public void Die()
    {
        if (deathDrop != null)
        {
            ExplodeDeathDrop();
        }
        gameObject.layer = 0;
        StartCoroutine(DeathEventDelay());
        currentHealth = 0;
        if (animator != null)
        {
            animator.SetTrigger("Death");
            animator.SetBool("Dead", true);
        }

        // Additional cleanup logic on death
        if (_rigidbody)
        {
            _rigidbody.velocity = Vector3.zero;
        }
        DisableComponentsOnDeath();
        if (GetComponent<EnemyControllerRB>())
        {
            ExplodeExperienceOrbs();
        }
        if (GetComponent<EnemyControllerRB>())
        {
            ExplodeCoffee();
        }
        StartCoroutine(DestroyOverTime(3f));
    }
    protected void ExplodeDeathDrop()
    {
        GameObject thing = Instantiate(deathDrop, neck.position, Quaternion.identity);
        Rigidbody thingrb = thing.GetComponent<Rigidbody>();
        thing.GetComponent<IDBadge>().deathEvent = deathDropPickupEvent;
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere.normalized;
        float explosionForce = UnityEngine.Random.Range(0.02f, 0.08f);
        thingrb.AddForce(randomDirection * explosionForce, ForceMode.Impulse);
    }
    protected void ExplodeCoffee()
    {
        for (int i = 0; i < coffeeAmount; i++)
        {
            if (UnityEngine.Random.Range(0.0f, 100.0f) <= coffeeDropChance)
            {
                GameObject coffee = Instantiate(coffeePrefab, neck.position, Quaternion.identity);
                Rigidbody coffeerb = coffee.GetComponent<Rigidbody>();

                Vector3 randomDirection = UnityEngine.Random.insideUnitSphere.normalized;
                float explosionForce = UnityEngine.Random.Range(0.1f, 0.3f);
                coffeerb.AddForce(randomDirection * explosionForce, ForceMode.Impulse);
            }
        }
    }
    protected void ExplodeNutsAndBolts()
    {
        AudioManager.instance.PlayCachedSound(AudioManager.instance.NutsAndBolts, transform.position, 1.0f);
        for (int i = 0; i < amountExploded; i++)
        {
            GameObject nut = Instantiate(nutsAndBolts[UnityEngine.Random.Range(0,nutsAndBolts.Length)], neck.position, Quaternion.identity);
            Rigidbody nutrb = nut.GetComponent<Rigidbody>();

            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere.normalized;
            float explosionForce = UnityEngine.Random.Range(0.1f, 0.3f);
            nutrb.AddForce(randomDirection * explosionForce, ForceMode.Impulse);
        }
    }
    private void ExplodeExperienceOrbs()
    {
        for (int i = 0; i < numOrbs; i++)
        {
            GameObject orb = Instantiate(experienceOrbPrefab, neck.position, Quaternion.identity);
            Rigidbody orbRb = orb.GetComponent<Rigidbody>();

            // Add random explosion force
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere.normalized;
            float explosionForce = UnityEngine.Random.Range(0.1f, 0.3f); // Random force
            orbRb.AddForce(randomDirection * explosionForce, ForceMode.Impulse);

            // Start the orb movement after a delay
            orb.GetComponent<ExperienceOrb>().StartFollowPlayer();
        }
    }

    private void DisableComponentsOnDeath()
    {
        if (skill != null)
        {
            if (skill.GetComponent<ShootSkill>() != null)
            {
                skill.GetComponent<ShootSkill>().DisableEverything();
            }
        }
        if (GetComponent<EnemyControllerRB>())
        {
            if (Objective1Manager.Instance != null)
            {
                Objective1Manager.Instance.currentEnemies -= 1;
            }
            Destroy(GetComponent<EnemyControllerRB>());
        }
        if (GetComponent<PathfindingScript>())
        {
            Destroy(GetComponent<PathfindingScript>());
        }
        if (GetComponent<EnemyAI>())
        {
            AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.EnemyDeathSounds[UnityEngine.Random.Range(0, AudioManager.instance.EnemyDeathSounds.Length)], transform.position);
            Destroy(GetComponent<EnemyAI>());
        }
        if (GetComponent<ThirdPersonControllerRB>())
        {
            AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.PlayerDeathSounds[UnityEngine.Random.Range(0, AudioManager.instance.PlayerDeathSounds.Length)], animator.transform.position);
            Destroy(GetComponent<ThirdPersonControllerRB>());
        }
        if (GetComponent<Rigidbody>()) 
        {
            GetComponent<Rigidbody>().velocity= Vector3.zero;
            GetComponent<Rigidbody>().useGravity = true;
        }
        if (GetComponent<EnemyHackable>())
        {
            Destroy(GetComponent<EnemyHackable>());
        }
        if (GetComponent<CapsuleCollider>())
        {
            Destroy(GetComponent<CapsuleCollider>());
        }
        gameObject.AddComponent<BoxCollider>().size = new Vector3(0.01f, 0.01f, 0.01f);
        gameObject.layer = 0;
    }

    public void Heal(float value)
    {
        currentHealth += value;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public float GetBaseDamage()
    {
        return baseDamage;
    }

    private void Update()
    {
        if (!canBeHit)
        {
            iFrameTimer += Time.deltaTime;
            if (iFrameTimer >= iFrameDuration)
            {
                iFrameTimer = 0;
                canBeHit = true;
            }
        }
        // Other logic, such as health checks or movement, goes here.
        if (GetComponent<PlayerHack>())
        {
            if (skill)
            {
                if (skill.GetCooldownRemaining() <= 0 && GetComponent<PlayerHack>().GetChargeValue() >= 10)
                {
                    IngameUIManager.Instance.GetComponent<Animator>().SetBool("CanUse", true);
                }
                else
                {
                    IngameUIManager.Instance.GetComponent<Animator>().SetBool("CanUse", false);
                }
            }
            else
            {
                IngameUIManager.Instance.GetComponent<Animator>().SetBool("CanUse", false);
            }
        }
    }

    // Coroutine to reset the trigger after a short delay
    IEnumerator ResetTriggerAfterDelay(string triggerName, float delay)
    {
    yield return new WaitForSeconds(delay);
    animator.ResetTrigger(triggerName);
    }

    private IEnumerator DestroyOverTime(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
