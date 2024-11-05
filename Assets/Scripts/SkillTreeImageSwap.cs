using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeImageSwap : MonoBehaviour
{
    public bool locked;
    [SerializeField]
    bool hack;
    [SerializeField]
    int level;
    [SerializeField]
    public int cost;
    [SerializeField]
    public string description;
    [SerializeField]
    Image mySprite;
    [SerializeField]
    Sprite unlockedImage;
    [SerializeField]
    Sprite lockedImage;
    private void Update()
    {
        if (hack)
        {
            if (SkillTreeManager.Instance.hackLevel >= level-1)
            {
                if (unlockedImage)
                    mySprite.sprite = unlockedImage;
                locked = false;
            }
            else
            {
                if (lockedImage)
                    mySprite.sprite = lockedImage;
                locked = true;
            }
        }
        else
        {
            if (SkillTreeManager.Instance.slashLevel >= level-1)
            {
                if (unlockedImage)
                    mySprite.sprite = unlockedImage;
                locked = false;
            }
            else
            {
                if (lockedImage)
                    mySprite.sprite = lockedImage;
                locked = true;
            }
        }
    }
    public void Press()
    {
        if (SkillTree.Instance.currentSelectedSkill != this && !locked)
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
