using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightHackable : Hackable
{
    [SerializeField]
    GameObject explosionPrefab;
    public List<Transform> ExplosionPoints;
    public float selfExplosionForce = 10;
    public float selfTorqueForce = 10;
    bool isActive = true;
    private void OnCollisionEnter(Collision collision)
    {
        if (!isActive)
            return;
        bool found = false;
        for (int i = 0; i < ExplosionPoints.Count;++i)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Default"))
            {
                Instantiate(explosionPrefab, ExplosionPoints[i].position, Quaternion.identity, null);
                found = true;
            }
        }
        AudioManager.instance.PlayCachedSound(AudioManager.instance.ExplosionSounds, transform.position, 0.6f);
        if (found)
            isActive = false;
    }
    public override void Hack(Entity player)
    {
        gameObject.AddComponent<Rigidbody>();
    }
}
