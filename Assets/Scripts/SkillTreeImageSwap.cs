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
    public Image mySprite;
    [SerializeField]
    Sprite unlockedImage;
    [SerializeField]
    Sprite lockedImage;
    public bool purchased = false;
    public GameObject purchasedGameObject;
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
            if (SkillTreeManager.Instance.hackLevel >= level)
            {
                purchased = true;
                purchasedGameObject.SetActive(true);
            }
            else
            {
                purchased = false;
                purchasedGameObject.SetActive(false);
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
            if (SkillTreeManager.Instance.slashLevel >= level)
            {
                purchased = true;
                purchasedGameObject.SetActive(true);
            }
            else
            {
                purchased = false;
                purchasedGameObject.SetActive(false);
            }
        }
    }
    public void Press()
    {
        if (locked)
            return;
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
