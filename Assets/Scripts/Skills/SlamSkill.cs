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
        {
            if (user.tag == "Player")
            {
                AudioManager.instance.PlayCachedSound(AudioManager.instance.MCSlamSkillBarks, user.transform.position, 1.0f);
            }
            else
            {
                AudioManager.instance.PlayCachedSound(AudioManager.instance.EnemySmashSounds, user.transform.position, 1.0f);
            }

            user.GetComponent<Animator>().SetBool("IgnoreStun", true);
            user.GetComponent<Animator>().SetTrigger("Smash");
        }

        StartCoroutine(Cooldown());
        return true;
    }
}