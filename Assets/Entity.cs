using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Entity : MonoBehaviour
{
    [SerializeField]
    protected float maxHealth = 100;
    [SerializeField]
    protected float currentHealth;
    [SerializeField]
    protected float baseDamage = 10;
    Animator animator;

    [SerializeField]
    string skillName;
    bool hasBeenHit = false;
    Vector3 hitDir;
    float knockBackAmount = 0;
    float hitDuration = 0.1f;
    float hitTimer = 0;

    bool selected;
    private Color originalTintColor = new Color(1.0f, 0f, 0f, 1.0f);
    private Color selectedTintColor = new Color(1.0f, 0f, 1.0f, 1.0f);

    public bool Selected
    {
        get
        {
            return selected;
        }

        set
        {
            if (selected != value)
            {
                selected = value;
                //Im unsure why this isn't working. placed fix in update but it is bad. hopefully can fix soon
                //if (selected)
                //{
                //    SetTintColor(selectedTintColor);
                //}
                //else
                //{
                //    SetTintColor(originalTintColor);
                //}
            }
        }
    }
    public void CopySkill(string skill)
    {
        skillName = skill;
    }
    public string GetSkillName()
    {
        return skillName;
    }
    public void SetTintColor(Color color)
    {
        SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer renderer in renderers)
        {
            if (renderer.material.HasProperty("_TintColor"))
            {
                renderer.material.SetColor("_TintColor", color);
            }
            if (renderer.material.HasProperty("_HighlightColor"))
            {
                renderer.material.SetColor("_HighlightColor", color);
            }
        }
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
    }

    public void TakeDamage(float value)
    {
        currentHealth -= value;
        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }
        if (currentHealth <= 0 )
        {
            currentHealth = 0;
            animator.SetTrigger("Death");
            animator.SetBool("Dead",true);

            if (GetComponent<EnemyController>())
            {
                Destroy(GetComponent<EnemyController>());
            }
            if (GetComponent<EnemyMovementScript>())
            {
                Destroy(GetComponent<EnemyMovementScript>());
            }
            if (GetComponent<ThirdPersonController>())
            {
                GetComponent<ThirdPersonController>().disableMovement = true;
            }
            if (GetComponent<CapsuleCollider>())
            {
                Destroy(GetComponent<CapsuleCollider>());
            }
            if (GetComponent<CharacterController>())
            {
                Destroy(GetComponent<CharacterController>());
            }
        }
    }

    public void TakeDamage(float value,Vector3 hitDirection, float kb)
    {
        hitDir = hitDirection;
        Debug.LogError("Take Hit "+ hitDirection);
        knockBackAmount = kb;
        currentHealth -= value;
        hitTimer = 0;
        hasBeenHit = true;
        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            animator.SetTrigger("Death");

            if (GetComponent<EnemyController>())
            {
                Destroy(GetComponent<EnemyController>());
            }
            if (GetComponent<EnemyMovementScript>())
            {
                Destroy(GetComponent<EnemyMovementScript>());
            }
            if (GetComponent<ThirdPersonController>())
            {
                GetComponent<ThirdPersonController>().disableMovement = true;
            }
            if (GetComponent<CapsuleCollider>())
            {
                Destroy(GetComponent<CapsuleCollider>());
            }
            if (GetComponent<CharacterController>())
            {
                Destroy(GetComponent<CharacterController>());
            }
        }
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
        //if (hasBeenHit)
        //{
        //    hitTimer += Time.deltaTime;
        //    CharacterController controller = GetComponent<CharacterController>();
        //    if (controller != null)
        //    {
        //        controller.Move(hitDir * knockBackAmount * Time.deltaTime);
        //    }
        //    if (hitTimer >= hitDuration)
        //    {
        //        hitTimer = 0;
        //        hasBeenHit = false;
        //    }
        //}
    }
    private void LateUpdate()
    {
        //temporary fix
        if (!GameManager.Instance.GetHackMode())
            return;
        if (selected)
        {
            SetTintColor(selectedTintColor);
        }
        else
        {
            SetTintColor(originalTintColor);
        }
    }
}
