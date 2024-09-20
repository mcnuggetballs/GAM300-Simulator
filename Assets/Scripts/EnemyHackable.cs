using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHackable : Hackable
{
    [SerializeField]
    string skillName;
    public override void Hack(Entity player)
    {
        player.GetComponent<AttackingScript>().skillName = skillName;
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


    protected override void LateUpdate()
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
