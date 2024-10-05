using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameLift : MonoBehaviour
{
    [SerializeField]
    LiftCollider openLiftCollider;
    [SerializeField]
    LiftCollider nextLevelCollider;
    Animator animator;
    [SerializeField]
    CinemachineVirtualCamera cameraController;
    bool changeScene = false;
    public LiftCollider GetOpenLiftCollider()
    {
        return openLiftCollider;
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    IEnumerator ChangeSceneEnumerator()
    {
        yield return new WaitForSeconds(1.5f);
        ChangeScene();
    }
    public void ChangeScene()
    {
        SceneTransition.Instance.SetTransitionMode(ScreenTransitionManager.TransitionMode.Fade);
        SceneTransition.Instance.TransitionToScene("Lift");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private Vector3 savedPosition;
    private Quaternion savedRotation;
    private Vector3 savedPosition2;
    private Quaternion savedRotation2;
    private void Update()
    {
        if (changeScene)
        {
            cameraController.Follow = null;
            cameraController.transform.position = savedPosition;
            cameraController.transform.rotation = savedRotation;
            Camera.main.transform.position = savedPosition2;
            Camera.main.transform.rotation = savedRotation2;
        }
        if (nextLevelCollider && nextLevelCollider.GetCollisions() >= 1)
        {
            ThirdPersonControllerRB playerRB = nextLevelCollider.GetPlayerInCollision();
            if (playerRB && changeScene == false)
            {
                playerRB.disableMovement = true;
                playerRB.StopMovement();
                animator.SetBool("Open", false);
                if (cameraController)
                {
                    savedPosition = cameraController.transform.position;
                    savedRotation = cameraController.transform.rotation;
                    savedPosition2 = Camera.main.transform.position;
                    savedRotation2 = Camera.main.transform.rotation;

                    cameraController.Follow = null;

                }
                StartCoroutine(ChangeSceneEnumerator());
                changeScene = true;
            }
        }
        else
        {
            if (openLiftCollider.GetCollisions() >= 1)
            {
                animator.SetBool("Open", true);
            }
            else
            {
                animator.SetBool("Open", false);
            }
        }
    }
}
