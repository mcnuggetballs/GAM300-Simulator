using UnityEngine;

public class HookProjectile : MonoBehaviour
{
    public float speed = 20f;                 // Speed of the projectile
    public float maxTravelDistance;           // Maximum distance the projectile can travel
    public LayerMask targetLayer;             // Layer for detecting targets
    private GameObject user;
    private Vector3 startPosition;

    private System.Action<GameObject, GameObject> onHitCallback;

    public void Initialize(GameObject user, float range, LayerMask targetLayer, System.Action<GameObject, GameObject> onHitCallback)
    {
        this.user = user;
        this.maxTravelDistance = range;
        this.targetLayer = targetLayer;
        this.onHitCallback = onHitCallback;
        startPosition = transform.position;
    }

    private void Update()
    {
        // Move the projectile forward
        transform.position += transform.forward * speed * Time.deltaTime;

        // Check for max travel distance
        if (Vector3.Distance(startPosition, transform.position) >= maxTravelDistance)
        {
            Destroy(gameObject); // Destroy if max range is reached
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the projectile hit the player layer
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            // Call the on-hit callback and pass the hit player and user
            onHitCallback?.Invoke(other.gameObject, user);

            // Destroy the projectile after hit
            Destroy(gameObject);
        }
    }
}