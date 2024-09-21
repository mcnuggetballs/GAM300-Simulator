using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverUIMessage : MonoBehaviour
{
    public string message;

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

}
