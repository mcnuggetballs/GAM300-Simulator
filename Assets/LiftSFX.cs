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

        // Start the coroutine to monitor SFX1 and trigger SFX2 once SFX1 is done
        StartCoroutine(PlaySFX2AfterSFX1());
    }

    IEnumerator PlaySFX2AfterSFX1()
    {
        // Wait until SFX1 is no longer playing
        while (sfx1.isPlaying)
        {
            yield return null; // Wait for the next frame
        }

        // Once SFX1 has finished, play SFX2 and set it to loop
        sfx2.loop = true;
        sfx2.Play();
    }
}
