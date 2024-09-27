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
    [SerializeField]
    Sprite green;
    [SerializeField]
    Sprite yellow;
    [SerializeField]
    Sprite red;
    [SerializeField]
    Image theID;
    [SerializeField]
    Sprite idGreen;
    [SerializeField]
    Sprite idYellow;
    [SerializeField]
    Sprite idRed;

    private void Update()
    {
        if (healthBar != null )
        {
            healthBar.fillAmount = theEntity.GetHealthFraction();
            if (healthBar.fillAmount >= 0.6666666666666667f)
            {
                theID.sprite = idGreen;
                healthBar.sprite = green;
            }
            else if (healthBar.fillAmount >= 0.3333333333333333f)
            {
                theID.sprite = idYellow;
                healthBar.sprite = yellow;
            }
            else
            {
                theID.sprite = idRed;
                healthBar.sprite = red;
            }
        }
    }
}
