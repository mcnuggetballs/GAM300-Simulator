using Cinemachine;
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
    public string tag = "";
    public void Spawn(Vector3 pos, float xSize, float ySize, float zSize, string tagVal = "")
    {
        transform.position = pos;
        transform.localScale = new Vector3(xSize,ySize,zSize);
        tag = tagVal;
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


                if (spawnedFrom != null && spawnedFrom.GetComponent<PlayerHack>())
                {
                    GameManager.Instance.TriggerSlowdown(0.075f, 0.2f);

                    spawnedFrom.GetComponent<PlayerHack>().AddChargeValue(spawnedFrom.GetComponent<PlayerHack>().chargeHitAmount);
                    if (tag == "Smash")
                    {
                        theEntity.TakeDamage(damage * SkillTreeManager.Instance.skillDamageMultiplier, hitDirection, 15);
                    }
                    else
                    {
                        theEntity.TakeDamage(damage * SkillTreeManager.Instance.slashDamageMultiplier, hitDirection, 15);
                    }
                    if (Random.Range(0.0f, 1.0f) < SkillTreeManager.Instance.hackEnergyChance)
                    {
                        spawnedFrom.GetComponent<PlayerHack>().AddChargeValue(1);
                    }
                }
                else
                {
                    theEntity.TakeDamage(damage, hitDirection, 15);
                }
                if(other.CompareTag("Player"))
                {
                    if (spawnedFrom.GetComponent<HookEnemyAI>() != null)
                    {
                        AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.EnemyHookSounds[Random.Range(1, AudioManager.instance.EnemyHookSounds.Length)], transform.position);
                    }
                    AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.MiscSounds[0],0.2f,other.transform.position,true);
                }
                else
                {
                    AudioManager.instance.PlayCachedSound(AudioManager.instance.HitSoundsFX,other.transform.position,0.2f);
                }
                VFXManager.Instance.Spawn("Hit_02", GetComponent<Collider>().ClosestPointOnBounds(other.bounds.center));

                CameraShake.ShakeSettings shakeSettings = new CameraShake.ShakeSettings
                {
                    duration = 0.1f,
                    shakeStrength = new Vector3(0.05f, 0.05f, 0.05f),
                    shakeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0),
                    shakeSpace = CameraShake.ShakeSpace.World,
                    shakePosition = transform.position,
                    maxDistance = 5f
                };

                GameManager.Instance.TriggerCameraShake(shakeSettings, Camera.main);
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
