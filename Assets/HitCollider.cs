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
                if(other.CompareTag("Player"))
                {
                    AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.MiscSounds[0],0.2f,other.transform.position,true);
                }
                else AudioManager.instance.PlayCachedSound(AudioManager.instance.HitSoundsFX,other.transform.position,0.3f);
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
