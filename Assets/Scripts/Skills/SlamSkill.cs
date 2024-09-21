using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlamSkill : Skill
{
    public override void Activate(GameObject user)
    {
        if (isOnCooldown) return;

        if (user.GetComponent<Animator>())
            user.GetComponent<Animator>().SetTrigger("Smash");

        StartCoroutine(Cooldown());
    }
}