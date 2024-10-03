using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelButtonToggle : ToggleButtonSprite
{
    [SerializeField]
    int level;
    public override void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        UpdateButtonState();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        UpdateButtonState();
    }

    public override void OnButtonClick()
    {
        isSelected = !isSelected;  // Toggle the selected state
        if (isSelected)
        {
            if (LevelSelectManager.Instance.selectedLevelButton)
            {
                LevelSelectManager.Instance.selectedLevelButton.OnButtonClick();
            }
            LevelSelectManager.Instance.level = level;
            LevelSelectManager.Instance.selectedLevelButton = this;
        }
        else
        {
            LevelSelectManager.Instance.level = -1;
            LevelSelectManager.Instance.selectedLevelButton = null;
        }
        UpdateButtonState();
    }
}
