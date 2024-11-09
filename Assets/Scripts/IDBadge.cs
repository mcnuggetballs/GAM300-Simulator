using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDBadge : MonoBehaviour
{
    float iFrameDuration = 1.5f;
    float iFrameTimer = 0.0f;
    bool canPickup = false;
    public EntityDeathEvent deathEvent;
    private void Update()
    {
        iFrameTimer += Time.deltaTime;
        if (iFrameTimer >= iFrameDuration)
        {
            canPickup = true;
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (!canPickup)
        {
            return;
        }
        // Check if the player has collided with the item
        if (other.gameObject.CompareTag("Player"))
        {
            deathEvent.Invoke();
            Destroy(gameObject);
        }
    }
}
