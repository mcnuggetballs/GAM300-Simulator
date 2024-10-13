using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillTreeButton : Button, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    public string description;
    [SerializeField]
    TMPro.TextMeshProUGUI descText;
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (descText != null)
        {
            descText.text = description;
        }
        Debug.Log("Hovering over button!");
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (descText != null)
        {
            descText.text = "";
        }
        Debug.Log("Stopped hovering over button!");
    }
}
