using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeePickup : MonoBehaviour
{
    public AudioClip pickupSFX; // Assign the sound effect in the inspector
    private AudioSource audioSource; // Audio source to play the sound

    void Start()
    {
        // Add an AudioSource component to the item if not already added
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    // This method will be called when something enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player has collided with the item
        if (other.CompareTag("Player"))
        {
            // Play the pickup sound effect
            if (pickupSFX != null)
            {
                audioSource.PlayOneShot(pickupSFX);
            }

            // Disable the item's renderer and collider to make it "disappear"
            GetComponent<Renderer>().enabled = false; // Hides the item
            GetComponent<Collider>().enabled = false; // Disables further collisions

            // Optionally, destroy the object after a delay to ensure SFX plays fully
            Destroy(gameObject, pickupSFX.length);
        }
    }
}

