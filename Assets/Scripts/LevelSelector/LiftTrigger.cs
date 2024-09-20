using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator LiftAnimator;
    private bool hasEntered;
    void Start()
    {
        
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
        if (hasEntered)
        {

            //switch to another scene in a few second
            //play lift sound
        }
    }
}
