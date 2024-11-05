using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeUIManager : MonoBehaviour
{
    [SerializeField]
    GameObject skillTree;
    [SerializeField]
    TMPro.TextMeshProUGUI orbCount;
    public void Toggle()
    {
        if (skillTree.activeSelf)
        {
            skillTree.SetActive(false);
        }
        else
        {
            skillTree.SetActive(true);
            SkillTree.Instance.currentSelectedSkill = null;
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
