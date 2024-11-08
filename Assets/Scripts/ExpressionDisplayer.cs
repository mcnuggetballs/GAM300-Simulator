using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpressionDisplayer : MonoBehaviour
{
    Animator animator;

    private void Start()
    {
        animator= GetComponent<Animator>();
    }

    public void Show()
    {
        animator.SetBool("Show", true);
    }

    public void Hide()
    {
        animator.SetBool("Show", false);
    }
}
