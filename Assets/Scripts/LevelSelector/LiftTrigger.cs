using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LiftTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Lift")]
    public Animator LiftAnimator;
    public static bool hasEntered;


    void Start()
    {
        hasEntered = false;
    }

	private void OnTriggerEnter(Collider other)
	{
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Triggered Lift");
            LiftAnimator.SetTrigger("PlayerHasEntered"); //when player enter the lift, lift door animation will be played
            hasEntered = true;
        }
	}

	// Update is called once per frame
	void Update()
    {
    }

}
