using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HitCollider : MonoBehaviour
{
    List<Collider> colliders = new List<Collider>();
    public float damage;
    public float spawnDuration = 0.1f;
    float timer = 0;
    public int parentLayer;
    public Vector3 hitDirection;
    public GameObject spawnedFrom;
    public void Spawn(Vector3 pos, float xSize, float ySize, float zSize)
    {
        transform.position = pos;
        transform.localScale = new Vector3(xSize,ySize,zSize);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!colliders.Contains(other))
        {
            colliders.Add(other);
            Entity theEntity = other.transform.GetComponent<Entity>();
            if (theEntity)
            {
                if (theEntity.gameObject.layer == parentLayer)
                    return;

                // Apply damage
                theEntity.TakeDamage(damage, hitDirection, 15);

                // Slow the game if the player hits the enemy
                if (spawnedFrom != null && spawnedFrom.GetComponent<PlayerHack>())
                {
                    // Trigger the slowdown effect for feedback
                    GameManager.Instance.TriggerSlowdown(0.075f, 0.2f);

                    // Add charge value to the player
                    spawnedFrom.GetComponent<PlayerHack>().AddChargeValue(spawnedFrom.GetComponent<PlayerHack>().chargeHitAmount);
                }

                // Spawn visual effects on hit
                VFXManager.Instance.Spawn("Hit_02", GetComponent<Collider>().ClosestPointOnBounds(other.bounds.center));
            }
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > spawnDuration)
        {
            Destroy(gameObject);
        }
    }
}
