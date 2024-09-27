using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillsUI : MonoBehaviour
{
    Entity entity;
    [SerializeField]
    Image iconImage;
    [SerializeField]
    Image fill;
    // Start is called before the first frame update
    void Start()
    {
        entity= GetComponent<Entity>();
    }

    // Update is called once per frame
    void Update()
    {
        if (entity != null)
        {
            if (entity.skill != null)
            {
                iconImage.sprite = entity.skill.icon;
                iconImage.color = Color.white;
                fill.fillAmount = (entity.skill.cooldownTime - entity.skill.GetCooldownRemaining())/entity.skill.cooldownTime;
            }
            else
            {
                iconImage.sprite = null;
                iconImage.color = Color.clear;
                fill.fillAmount = 0;
            }
        }
    }
}
