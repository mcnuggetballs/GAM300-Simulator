using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
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
        Collider playerCollider = null;
        foreach (Collider col in collisions)
        {
            if (col == null)
                continue;
            if (col.GetComponent<ThirdPersonControllerRB>())
            {
                playerCollider = col;
            }
        }
        if (playerCollider)
        {
            return playerCollider.GetComponent<ThirdPersonControllerRB>();
        }
        return null;
    }
}
