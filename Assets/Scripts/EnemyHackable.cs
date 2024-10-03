using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHackable : Hackable
{
    public override void Hack(Entity player)
    {
        player.GetComponent<Entity>().skill = Instantiate(GetComponent<Entity>().skill);
    }

    public Skill GetEnemySkill()
    {
        return GetComponent<Entity>().skill;
    }

    protected override void SetTintColor(Color color)
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
}
