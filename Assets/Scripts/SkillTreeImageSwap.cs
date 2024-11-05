using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeImageSwap : MonoBehaviour
{
    [SerializeField]
    bool hack;
    [SerializeField]
    int level;
    [SerializeField]
    int cost;
    [SerializeField]
    Image mySprite;
    [SerializeField]
    Sprite unlockedImage;
    [SerializeField]
    Color unlockedColor;
    private void Update()
    {
        if (hack)
        {
            if (SkillTreeManager.Instance.hackLevel >= level)
            {
                if (unlockedImage)
                    mySprite.sprite = unlockedImage;
                mySprite.color = unlockedColor;
            }
        }
        else
        {
            if (SkillTreeManager.Instance.slashLevel >= level)
            {
                if (unlockedImage)
                    mySprite.sprite = unlockedImage;
                mySprite.color = unlockedColor;
            }
        }
    }
    public void Press()
    {
        if (SkillTree.Instance.currentSelectedSkill != this)
        {
            SkillTree.Instance.currentSelectedSkill = this;
        }
        else
        {
            SkillTree.Instance.currentSelectedSkill = null;
        }
    }
    public void Buy()
    {
        if (hack)
            SkillTreeManager.Instance.LevelUpHack(level,cost);
        else
            SkillTreeManager.Instance.LevelUpSlash(level, cost);
    }
}
