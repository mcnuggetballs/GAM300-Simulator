using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class OpenBriefScript : MonoBehaviour
{
    [SerializeField]
    Animator playerAnimator;
    [SerializeField]
    Animator briefCaseAnimator;
    [SerializeField]
    CinemachineVirtualCamera mainCamera;
    [SerializeField]
    CinemachineVirtualCamera suitcaseCamera;

    void Start()
    {
        playerAnimator = GetComponent<Animator>();  
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            playerAnimator.SetBool("OpenBrief", !playerAnimator.GetBool("OpenBrief"));
            briefCaseAnimator.SetBool("Open", !briefCaseAnimator.GetBool("Open"));
            mainCamera.enabled = !playerAnimator.GetBool("OpenBrief");
            suitcaseCamera.enabled = playerAnimator.GetBool("OpenBrief");
        }
    }
}
