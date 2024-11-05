using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeUIManager : MonoBehaviour
{
    [SerializeField]
    GameObject skillTree;
    [SerializeField]
    TMPro.TextMeshProUGUI theText;
    [SerializeField]
    TMPro.TextMeshProUGUI orbCount;
    public void Toggle()
    {
        if (skillTree.activeSelf)
        {
            skillTree.SetActive(false);
            theText.text = "Skill Tree";
        }
        else
        {
            skillTree.SetActive(true);
            SkillTree.Instance.currentSelectedSkill = null;
            theText.text = "Close";
        }
    }

    private void Update()
    {
        if (orbCount)
        {
            orbCount.text = GameManager.Instance.GetXP().ToString();
        }
    }
}
