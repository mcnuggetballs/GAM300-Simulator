using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    [Header("Level Buttons")]
    [SerializeField]
    private List<GameObject> level1;
    [SerializeField]
    private List<GameObject> level2;
    [SerializeField]
    private List<GameObject> level3;
    [SerializeField]
    private GameObject ButtonPenthouse;

    [Header("Level Counter")]
    [SerializeField]
    private int currentLevel = 1;

    [Header("UI Hover")]
    [SerializeField]
    private GameObject HoverPanel;
    [SerializeField]
    private TextMeshProUGUI HoverTextUI;
    private RectTransform PanelTransform;

    [Header("Set Text in Level")]
    private string text1;
    private string text2;
    private string text3;


    // Start is called before the first frame update
    void Start()
    {
        PanelTransform = HoverPanel.GetComponent<RectTransform>();
        HoverPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnMouseOver()
	{
		
	}

    public void ShowHoverUI(string textToDisplay)
    {
        HoverTextUI.text = textToDisplay;
        HoverPanel.SetActive(true);
    }

    public void HideHoverUI()
    {
        HoverPanel.SetActive(false);
    }

    //reload scene for a new level
    public void SwitchLevel(int level)
    {
        switch (level)
        {
            case 1:
                currentLevel = 1;
                break;
            case 2:
                currentLevel = 2;
                break;
            case 3:
                currentLevel = 3;
                break;
        }
    }
}
