using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillTree : MonoBehaviour
{
    public SkillTreeImageSwap currentSelectedSkill;

    public GameObject outline;
    public TMPro.TextMeshProUGUI desc;
    public static SkillTree Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (currentSelectedSkill != null)
        {
            outline.SetActive(true);
            outline.transform.position = currentSelectedSkill.transform.position;
            desc.text = currentSelectedSkill.description;
        }
        else
        {
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
