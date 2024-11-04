using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSecond : MonoBehaviour
{
    [SerializeField]
    float seconds = 5.0f;
    private void Start()
    {
        Destroy(gameObject,seconds);
    }
}
