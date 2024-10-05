using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToggleButtonSprite : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    protected Button button;
    protected bool isHovered = false;
    protected bool isSelected = false;

    // Sprites for different states
    public Sprite normalSprite;    // Sprite when not hovered or selected
    public Sprite hoverSprite;     // Sprite when hovered
    public Sprite selectedSprite;  // Sprite when selected
    public bool IsSelected
    {
        get { return isSelected; }
        set { isSelected = value; }
    }
    protected virtual void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);

        // Set initial sprite
        UpdateButtonState();
    }

    // When the pointer enters the button (hover state)
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        UpdateButtonState();
    }

    // When the pointer exits the button (no longer hovered)
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        UpdateButtonState();
    }

    // This function is called when the button is clicked (to toggle selection)
    public virtual void OnButtonClick()
    {
        isSelected = !isSelected;  // Toggle the selected state
        UpdateButtonState();
    }

    public void Deselect()
    {
        isSelected = false;
        UpdateButtonState();
    }
    public void Select()
    {
        isSelected = true;
        UpdateButtonState();
    }

    protected virtual void UpdateButtonState()
    {
        // Determine the sprite based on hover and selection states
        if (isSelected)
        {
            // Set the selected sprite if the button is selected
            button.image.sprite = selectedSprite;
        }
        else if (isHovered)
        {
            // Set the hover sprite if the button is hovered but not selected
            if (hoverSprite)
                button.image.sprite = hoverSprite;
        }
        else
        {
            // Set the normal sprite when neither selected nor hovered
            button.image.sprite = normalSprite;
        }
    }
}
