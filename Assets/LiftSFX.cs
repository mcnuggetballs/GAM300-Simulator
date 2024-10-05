using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftSFX : MonoBehaviour
{
    public AudioSource sfx1;  // Reference to SFX1 (plays once)
    public AudioSource sfx2;  // Reference to SFX2 (loops)

    void Start()
    {
        // Play SFX1 first
        sfx1.Play();

        // Subscribe to the event when SFX1 finishes playing
        StartCoroutine(WaitForSFX1ToFinish());
    }

    // Coroutine to wait for SFX1 to finish
    IEnumerator WaitForSFX1ToFinish()
    {
        // Wait for the duration of SFX1
        yield return new WaitForSeconds(sfx1.clip.length);

        // Play SFX2 and set it to loop
        sfx2.loop = true;
        sfx2.Play();
    }
}
