using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    [SerializeField]
    Entity theEntity;
    [SerializeField]
    Image healthBar;

    private void Update()
    {
        if (healthBar != null )
        {
            healthBar.fillAmount = theEntity.GetHealthFraction();
            if (healthBar.fillAmount >= 0.75f)
            {
                healthBar.color = Color.green;
            }
            else if (healthBar.fillAmount >= 0.5f)
            {
                healthBar.color = new Color((float)139/ (float)255, (float)195 / (float)255, (float)74 / (float)255);
            }
            else if (healthBar.fillAmount >= 0.25f)
            {
                healthBar.color = Color.yellow;
            }
            else
            {
                healthBar.color = Color.red;
            }
        }
    }
}
