using UnityEngine;
using System.Collections;

public class Exploder : MonoBehaviour
{
    public float explosionTime = 0;
    public float randomizeExplosionTime = 0;
    public float radius = 15;
    public float power = 1;
    public int probeCount = 150;
    public float explodeDuration = 0.5f;
    public LayerMask layerMask; // Add a LayerMask to filter the layers
    protected bool exploded = false;
    [SerializeField]
    float damage = 30;

    public virtual IEnumerator explode()
    {
        ExploderComponent[] components = GetComponents<ExploderComponent>();
        foreach (ExploderComponent component in components)
        {
            if (component.enabled)
            {
                component.onExplosionStarted(this);
            }
        }
        disableCollider();
        for (int i = 0; i < probeCount; i++)
        {
            shootFromCurrentPosition(i);
        }
        enableCollider();
        yield return new WaitForFixedUpdate();
    }

    protected virtual void shootFromCurrentPosition()
    {
        Vector3 probeDir = Random.onUnitSphere;
        Ray testRay = new Ray(transform.position, probeDir);
        shootRay(testRay, radius);
    }

    protected virtual void shootFromCurrentPosition(int i)
    {
        Ray testRay = new Ray(transform.position, GetUniformDirection(i,probeCount));
        shootRay(testRay, radius);
    }

    protected bool wasTrigger;
    public virtual void disableCollider()
    {
        if (GetComponent<Collider>())
        {
            wasTrigger = GetComponent<Collider>().isTrigger;
            GetComponent<Collider>().isTrigger = true;
        }
    }

    public virtual void enableCollider()
    {
        if (GetComponent<Collider>())
        {
            GetComponent<Collider>().isTrigger = wasTrigger;
        }
    }

    protected virtual void init()
    {
        power *= 500000;

        if (randomizeExplosionTime > 0.01f)
        {
            explosionTime += Random.Range(0.0f, randomizeExplosionTime);
        }
    }

    void Start()
    {
        init();
    }

    void FixedUpdate()
    {
        if (Time.time > explosionTime && !exploded)
        {
            exploded = true;
            StartCoroutine("explode");
        }
    }

    private void shootRay(Ray testRay, float estimatedRadius)
    {
        RaycastHit hit;
        // Use the layerMask to filter the raycast
        if (Physics.Raycast(testRay, out hit, estimatedRadius, layerMask))
        {
            Entity theEntity = hit.collider.GetComponent<Entity>();
            if (hit.rigidbody != null || theEntity)
            {
                if (theEntity != null)
                {
                    theEntity.TakeDamage(damage);
                }
                if (hit.rigidbody)
                {
                    hit.rigidbody.AddForceAtPosition(power * Time.deltaTime * testRay.direction / probeCount, hit.point);
                    estimatedRadius /= 2;
                }

            }
            else
            {
                Debug.LogError(hit.collider);
                Vector3 reflectVec = Random.onUnitSphere;
                if (Vector3.Dot(reflectVec, hit.normal) < 0)
                {
                    reflectVec *= -1;
                }
                Ray emittedRay = new Ray(hit.point, reflectVec);
                shootRay(emittedRay, estimatedRadius - hit.distance);
            }
        }
    }

    private Vector3 GetUniformDirection(int i, int totalCount)
    {
        float phi = Mathf.Acos(1 - 2 * (i + 0.5f) / totalCount);
        float theta = Mathf.PI * (1 + Mathf.Sqrt(5)) * i;

        float x = Mathf.Sin(phi) * Mathf.Cos(theta);
        float y = Mathf.Sin(phi) * Mathf.Sin(theta);
        float z = Mathf.Cos(phi);

        return new Vector3(x, y, z).normalized;
    }
}
