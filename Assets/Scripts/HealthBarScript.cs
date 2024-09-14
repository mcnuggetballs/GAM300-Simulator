using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    [SerializeField]
    Image health;
    public Entity theEntity;
    // Update is called once per frame
    void Update()
    {
        health.fillAmount = theEntity.GetHealthFraction();
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }
}