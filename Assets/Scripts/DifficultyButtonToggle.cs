using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DifficultyButtonToggle : ToggleButtonSprite
{
    [SerializeField]
    int difficulty;
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
        isSelected = !isSelected;
        if (isSelected)
        {
            if (LevelSelectManager.Instance.selectedDifficultyButton)
            {
                LevelSelectManager.Instance.selectedDifficultyButton.OnButtonClick();
            }
            LevelSelectManager.Instance.difficulty = difficulty;
            LevelSelectManager.Instance.selectedDifficultyButton = this;
        }
        else
        {
            LevelSelectManager.Instance.difficulty = -1;
            LevelSelectManager.Instance.selectedDifficultyButton = null;
        }
        UpdateButtonState();
    }
}
