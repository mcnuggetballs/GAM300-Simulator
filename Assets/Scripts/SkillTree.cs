using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillTree : MonoBehaviour
{
    public SkillTreeImageSwap currentSelectedSkill;

    public GameObject outline;
    public Image skillIcon;
    public TMPro.TextMeshProUGUI desc;
    public TMPro.TextMeshProUGUI cost;
    public static SkillTree Instance { get; private set; }
    Animator animator;
    private void Awake()
    {
        Instance = this;
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (currentSelectedSkill != null)
        {
            skillIcon.sprite = currentSelectedSkill.mySprite.sprite;
            animator.SetBool("Showing",true);
            outline.SetActive(true); 
            outline.transform.parent = currentSelectedSkill.transform;
            outline.transform.localPosition = Vector3.zero;
            desc.text = currentSelectedSkill.description;
            cost.text = currentSelectedSkill.cost.ToString();
        }
        else
        {
            animator.SetBool("Showing", false);
            outline.SetActive(false);
        }
    }
    public void PurchaseButton()
    {
        if (currentSelectedSkill != null)
        {
            currentSelectedSkill.Buy();
        }
    }
}
