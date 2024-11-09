using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GantryOpen : MonoBehaviour
{
    [SerializeField]
    List<Animator> gantries;

    public void OpenGantries()
    {
        foreach (var anim in gantries)
        {
            anim.SetBool("Open", true);
        }
    }
}
