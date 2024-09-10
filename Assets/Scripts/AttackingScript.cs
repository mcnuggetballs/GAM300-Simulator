using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingScript : MonoBehaviour
{
    [SerializeField]
    protected Animator anim;
    public static int noOfClicks = 0;
    protected ThirdPersonController controller;
    private void Start()
    {
        if (controller == null)
        {
            controller = GetComponent<ThirdPersonController>();
        }
    }
    // Update is called once per frame
    public void ResetClicks()
    {
        noOfClicks = 0;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClick();
        }
    }
    void OnClick()
    {
        noOfClicks++;
        if (noOfClicks == 1)
        {
            anim.SetBool("Hit1", true);
        }
        noOfClicks = Mathf.Clamp(noOfClicks, 0, 3);
        if (noOfClicks >= 2 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.25f && anim.GetCurrentAnimatorStateInfo(0).IsName("Hit1"))
        {
            anim.SetBool("Hit2", true);
            anim.SetBool("Hit1", false);
        }
        if (noOfClicks >= 3 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.25f && anim.GetCurrentAnimatorStateInfo(0).IsName("Hit2"))
        {
            anim.SetBool("Hit3", true);
            anim.SetBool("Hit2", false);
        }
    }
}
