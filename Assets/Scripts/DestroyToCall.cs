using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyToCall : MonoBehaviour
{
    public void DestroyMe()
    {
        Destroy(gameObject);
    }
}
