using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HowToPlayPageControl : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI numberText;
    [SerializeField]
    Scrollbar scrollBar;
    private void Update()
    {
        if (scrollBar)
        {
            if (numberText)
            {
                if (scrollBar.value <= (1.0f / 3.0f))
                {
                    numberText.text = "3";
                }
                else if (scrollBar.value <= (2.0f / 3.0f))
                {
                    numberText.text = "2";
                }
                else if (scrollBar.value <= (3.0f / 3.0f))
                {
                    numberText.text = "1";
                }
                else
                {
                    numberText.text = "1";
                }
            }
        }
    }
}
