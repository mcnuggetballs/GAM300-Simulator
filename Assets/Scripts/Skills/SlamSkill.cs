using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlamSkill : Skill
{
    public override bool Activate(GameObject user)
    {
        if (user.GetComponent<Animator>() && !user.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle Walk Run Blend"))
            return false;
        if (isOnCooldown)
            return false;

        if (user.GetComponent<Animator>())
            user.GetComponent<Animator>().SetTrigger("Smash");

        StartCoroutine(Cooldown());
        return true;
    }
}