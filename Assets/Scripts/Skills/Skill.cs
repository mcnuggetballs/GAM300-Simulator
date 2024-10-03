using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    public string skillName;
    public Sprite icon;
    public Sprite iconFill;
    public Sprite iconBorder;
    public Sprite iconTriangle;
    public float cooldownTime;
    protected bool isOnCooldown = false;
    protected float cooldownRemaining = 0f;
    public abstract bool Activate(GameObject user);

    public virtual IEnumerator Cooldown()
    {
        isOnCooldown = true;
        cooldownRemaining = cooldownTime;

        while (cooldownRemaining > 0)
        {
            cooldownRemaining -= Time.deltaTime;
            yield return null;
        }

        cooldownRemaining = 0f;
        isOnCooldown = false;
    }

    public float GetCooldownRemaining()
    {
        return cooldownRemaining;
    }
}