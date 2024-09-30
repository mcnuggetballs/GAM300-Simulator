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
                theEntity.TakeDamage(damage, hitDirection,15);

                if (spawnedFrom!= null)
                {
                    if (spawnedFrom.GetComponent<PlayerHack>())
                    {
                        spawnedFrom.GetComponent<PlayerHack>().AddChargeValue(spawnedFrom.GetComponent<PlayerHack>().chargeHitAmount);
                    }
                }

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
