using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftCollider : MonoBehaviour
{
    List<Collider> collisions = new List<Collider>();
    private void OnTriggerEnter(Collider other)
    {
        if (!collisions.Contains(other))
        {
            collisions.Add(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (collisions.Contains(other))
        {
            collisions.Remove(other);
        }
    }

    public int GetCollisions()
    {
        return collisions.Count;
    }

    public ThirdPersonControllerRB GetPlayerInCollision()
    {
        Collider playerCollider = collisions.Find(item => item.GetComponent<ThirdPersonControllerRB>());
        if (playerCollider)
        {
            return playerCollider.GetComponent<ThirdPersonControllerRB>();
        }
        return null;
    }
}
