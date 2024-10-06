using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderHoverDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private Slider slider;
    private bool isHovering = false;
    [SerializeField] private UIbuttonsfx uiButtonSFX;

    void Start()
    {
        slider = GetComponent<Slider>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        uiButtonSFX.PlayHoverSFX();
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        uiButtonSFX.PlayClickSFX();
    }
}
