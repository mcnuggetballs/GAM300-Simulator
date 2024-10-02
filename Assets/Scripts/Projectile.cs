using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private GameObject hitEffect;

    private Vector3 _direction;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Initialize(Vector3 direction, GameObject spawnedFrom)
    {
        _direction = direction.normalized;

        if (spawnedFrom.layer == LayerMask.NameToLayer("Enemy"))
        {
            gameObject.layer = LayerMask.NameToLayer("EnemyProjectile");
        }
        else if (spawnedFrom.layer == LayerMask.NameToLayer("Player"))
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerProjectile");
        }
    }

    private void Update()
    {
        transform.position += _direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Entity hitEntity = other.GetComponent<Entity>();
        if (hitEntity != null)
        {

            hitEntity.TakeDamage(damage);

            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }
            CameraShake.ShakeSettings shakeSettings = new CameraShake.ShakeSettings
            {
                duration = 0.2f,
                shakeStrength = new Vector3(0.15f, 0.15f, 0.15f),
                shakeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0),
                shakeSpace = CameraShake.ShakeSpace.World,
                shakePosition = transform.position,
                maxDistance = 8f
            };
            GameManager.Instance.TriggerCameraShake(shakeSettings, Camera.main);
            Destroy(gameObject);
        }
        else
        {
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            CameraShake.ShakeSettings shakeSettings = new CameraShake.ShakeSettings
            {
                duration = 0.2f,
                shakeStrength = new Vector3(0.15f, 0.15f, 0.15f),
                shakeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0),
                shakeSpace = CameraShake.ShakeSpace.World,
                shakePosition = transform.position,
                maxDistance = 8f
            };
            GameManager.Instance.TriggerCameraShake(shakeSettings, Camera.main);
            Destroy(gameObject);
        }
    }
}
