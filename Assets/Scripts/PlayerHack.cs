using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHack : MonoBehaviour
{
    public KeyCode hackKey;
    void Update()
    {
        if (Input.GetKeyDown(hackKey))
        {
            GameManager.Instance.ToggleHackMode(!GameManager.Instance.GetHackMode());
        }
    }
}
