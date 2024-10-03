using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hackable : MonoBehaviour
{
    public bool hacked = false;
    public bool isClose = false;
    protected Color originalTintColor = new Color(1.0f, 0f, 0f, 1.0f);
    protected Color closeTintColor = new Color(1.0f, 0.5f, 0f, 1.0f);
    protected Color selectedTintColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
    protected Color hackedTintColor = new Color(0.0f,0.0f,0.0f,1.0f);
    protected bool selected;
    PlayerHack playerHack;
    public bool Selected
    {
        get
        {
            return selected;
        }

        set
        {
            if (selected != value)
            {
                selected = value;
                //Im unsure why this isn't working. placed fix in update but it is bad. hopefully can fix soon
                //if (selected)
                //{
                //    SetTintColor(selectedTintColor);
                //}
                //else
                //{
                //    SetTintColor(originalTintColor);
                //}
            }
        }
    }
    public virtual void Hack(Entity player)
    {

    }


    protected virtual void SetTintColor(Color color)
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.material.HasProperty("_TintColor"))
            {
                renderer.material.SetColor("_TintColor", color);
            }
            if (renderer.material.HasProperty("_HighlightColor"))
            {
                renderer.material.SetColor("_HighlightColor", color);
            }
        }
    }

    protected virtual void LateUpdate()
    {
        playerHack = FindAnyObjectByType<PlayerHack>();
        if (playerHack != null)
        {
            float distanceToCam = Vector3.Distance(transform.position, playerHack.GetComponent<Entity>().neck.transform.position);
            if (distanceToCam < playerHack.sphereCastDistance)
            {
                isClose = true;
            }
            else
            {
                isClose = false;
            }
        }
        //temporary fix
        if (!GameManager.Instance.GetHackMode())
            return;
        if (selected)
        {
            if (hacked)
            {
                SetTintColor(hackedTintColor);
            }
            else
            {
                SetTintColor(selectedTintColor);
            }
        }
        else
        {
            if (hacked)
            {
                SetTintColor(hackedTintColor);
            }
            else if (isClose)
            {
                SetTintColor(closeTintColor);
            }
            else
            {
                SetTintColor(originalTintColor);
            }
        }
    }
}
