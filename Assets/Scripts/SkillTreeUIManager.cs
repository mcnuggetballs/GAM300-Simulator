using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeUIManager : MonoBehaviour
{
    [SerializeField]
    GameObject skillTree;
    [SerializeField]
    TMPro.TextMeshProUGUI orbCount;
    [SerializeField]
    Animator maskAnimator;
    public void Show()
    {
        if (maskAnimator)
        {
            maskAnimator.SetBool("Show", true);
        }
    }

    public void Hide()
    {
        maskAnimator.SetBool("Show", false);
        SkillTree.Instance.currentSelectedSkill = null;
    }

    private void Update()
    {
        if (orbCount)
        {
            orbCount.text = GameManager.Instance.GetXP().ToString();
        }
    }
}
