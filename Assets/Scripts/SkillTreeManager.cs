using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillTreeManager
{
    private static SkillTreeManager _instance;

    public static SkillTreeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SkillTreeManager();
            }
            return _instance;
        }
    }

    public float hackTimeMultiplier = 1.0f;
    public float skillDamageMultiplier = 1.0f;

    public float slashDamageMultiplier = 1.0f;
    public float hackEnergyChance = 0.0f;

    public int hackLevel = 0;
    public int slashLevel = 0;
    public void LevelUpHack(int value,int cost)
    {
        if (GameManager.Instance.GetXP() < cost)
            return;
        switch (value)
        {
            case 1:
                Debug.LogError("HackLvl " + hackLevel);
                if (hackLevel!= 0)
                {
                    return;
                }
                hackLevel = 1;
                hackTimeMultiplier = 0.5f;
                GameManager.Instance.MinusExperience(cost);
                break;
            case 2:
                if (hackLevel != 1)
                {
                    return;
                }
                hackLevel = 2;
                skillDamageMultiplier = 1.5f;
                GameManager.Instance.MinusExperience(cost);
                break;
            case 3:
                if (hackLevel != 2)
                {
                    return;
                }
                hackLevel = 3;
                hackTimeMultiplier = 0.0f;
                skillDamageMultiplier = 2.0f;
                GameManager.Instance.MinusExperience(cost);
                break;
            default:
            break;
        }
    }
    public void LevelUpSlash(int value, int cost)
    {
        if (GameManager.Instance.GetXP() < cost)
            return;
        switch (value)
        {
            case 1:
                if (slashLevel != 0)
                {
                    return;
                }
                slashLevel = 1;
                hackEnergyChance = 0.25f;
                GameManager.Instance.MinusExperience(cost);
                break;
            case 2:
                if (slashLevel != 1)
                {
                    return;
                }
                slashLevel = 2;
                slashDamageMultiplier = 1.5f;
                GameManager.Instance.MinusExperience(cost);
                break;
            case 3:
                if (slashLevel != 2)
                {
                    return;
                }
                slashLevel = 3;
                hackEnergyChance = 0.5f;
                slashDamageMultiplier = 2.0f;
                GameManager.Instance.MinusExperience(cost);
                break;
            default:
                break;
        }
    }
}
