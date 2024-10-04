using System.Collections;
using UnityEngine;

public class StationaryShooterAI : EnemyAI
{
    Animator animator;
    [SerializeField] private float shootCooldown = 2f;

    private float _timeSinceLastShot = 0f;

    protected override void Start()
    {
        base.Start();
        _currentState = State.Chasing;
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Hurt"))
        {
            _timeSinceLastShot += Time.deltaTime;
        }
        base.Update();
    }

    protected override void Patrol()
    {
        if (_playerDetected = DetectPlayer())
        {
            AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.EnemyAggroSounds[Random.Range(0, AudioManager.instance.EnemyAggroSounds.Length - 1)], transform.position);
            _currentState = State.Chasing;
        }
    }

    protected override bool DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        foreach (var hit in hits)
        {
            if (hit.transform == player)
            {
                if (HasLineOfSight())
                {
                    _lastKnownPlayerPosition = player.position;
                    return true;
                }
            }
        }
        return false;
    }

    protected override void Chase()
    {
        if (HasLineOfSight())
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _lastKnownPlayerPosition);

            if (distanceToPlayer <= attackRadius)
            {
                _currentState = State.Attacking;
            }
        }
    }
    protected override bool HasLineOfSight()
    {
        Vector3 directionToPlayer = (player.GetComponent<Entity>().neck.position - GetComponent<Entity>().neck.position).normalized;
        if (!Physics.Raycast(GetComponent<Entity>().neck.position, directionToPlayer, Vector3.Distance(GetComponent<Entity>().neck.position, player.GetComponent<Entity>().neck.position), obstacleLayer))
        {
            _lastKnownPlayerPosition = player.GetComponent<Entity>().neck.position;
            return true;
        }
        return false;
    }

    protected override void Attack()
    {
        if (GetComponent<EnemyControllerRB>() != null)
        {
            GetComponent<EnemyControllerRB>().SetLookDirection((player.position - transform.position).normalized);
        }

        if (_timeSinceLastShot >= shootCooldown)
        {

            if (theEntity)
            {
                if (theEntity.skill)
                    theEntity.skill.Activate(gameObject);
            }
            _timeSinceLastShot = 0f;
            _currentState = State.Chasing;
        }
    }
}
