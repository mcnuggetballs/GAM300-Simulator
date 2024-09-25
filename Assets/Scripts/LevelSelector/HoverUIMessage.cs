using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverUIMessage : MonoBehaviour
{
    public string message;
    public LayerMask ButtonLayer;  // You can assign specific layers for clickable objects in the Inspector

    private void OnMouseEnter()
	{
		Canvas.ForceUpdateCanvases();
		HoverUI._instance.SetAndShowUI(message);
	}

	private void OnMouseExit()
	{
		Canvas.ForceUpdateCanvases();
		HoverUI._instance.HideUI();
	}

    public void ButtonCheck(string ButtonMode)
    {
        switch (ButtonMode)
        {
            case "Level_1_Button_1_Easy":
                Debug.Log("Button Level 1 Easy clicked");
                LevelSelector.SwitchLevel(1);
                //LevelLoader.CurrentLevel = 1;
                break;
            case "Level_2_Button_Easy":
                Debug.Log("Button Level 2 Easy clicked");
                LevelSelector.SwitchLevel(2);
                //LevelLoader.CurrentLevel = 2;
                break;
            case "Level_3_Button_Easy":
                Debug.Log("Button Level 3 Easy clicked");
                LevelSelector.SwitchLevel(3);
                //LevelLoader.CurrentLevel = 3;
                break;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ButtonLayer))
            {
                if (hit.collider != null)
                {
                    ButtonCheck(hit.collider.gameObject.name);
                }
            }
        }
    }
}
