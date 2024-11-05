using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    public SkillTreeImageSwap currentSelectedSkill;

    public GameObject outline;
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
        }
        else
        {
            outline.SetActive(false);
        }
    }
}
