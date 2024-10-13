using System.Collections;
using UnityEngine;

public class ExperienceOrb : MonoBehaviour
{
    public float followDelay = 3f;
    public float followSpeed = 5f;
    private Transform player;
    private Entity playerEntity;

    private Rigidbody rb;
    private Collider theCollider;
    private bool isFollowing = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = FindObjectOfType<PlayerHack>().transform;
        if (player)
        {
            playerEntity = player.GetComponent<Entity>();
        }
        theCollider = GetComponent<Collider>();
    }

    public void StartFollowPlayer()
    {
        StartCoroutine(FollowPlayerAfterDelay());
    }

    private IEnumerator FollowPlayerAfterDelay()
    {
        yield return new WaitForSeconds(followDelay);
        theCollider.isTrigger = true;
        isFollowing = true;
    }

    private void Update()
    {
        if (isFollowing)
        {
            Vector3 direction = (playerEntity.neck.position - transform.position).normalized;
            rb.velocity = direction * followSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerHack>())
        {
            GameManager.Instance.AddExperience(1);
            Destroy(gameObject);
        }
    }
}
