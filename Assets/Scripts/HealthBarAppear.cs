using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HealthBarAppear : MonoBehaviour
{
    [SerializeField]
    Entity theEntity;
    [SerializeField]
    GameObject theHealthBar;
    [SerializeField]
    float displayDuration = 2.0f;
    float displayTimer;

    float currentHealth;
    private void Start()
    {
        currentHealth = theEntity.GetCurrentHealth();
        displayTimer = displayDuration;
        DisableHealthBar();
    }

    void Update()
    {
        if (currentHealth != theEntity.GetCurrentHealth())
        {
            currentHealth = theEntity.GetCurrentHealth();
            ShowHealthBar();
        }
        if (displayTimer <= displayDuration)
        {
            displayTimer += Time.deltaTime;
        }
        else
        {
            DisableHealthBar();
        }
    }

    void ShowHealthBar()
    {
        if (theHealthBar != null)
        {
            theHealthBar.SetActive(true);
            displayTimer= 0;
        }
    }

    void DisableHealthBar()
    {
        if (theHealthBar != null)
        {
            theHealthBar.SetActive(false);
        }
    }
}
