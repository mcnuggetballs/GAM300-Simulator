using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PathfindingScript : MonoBehaviour
{
    public EnemyControllerRB enemyController;
    private bool foundPath = false;
    private NavMeshAgent _navMeshAgent;
    private Vector3[] _path;
    private int _currentPathIndex = 0;

    // Variables for handling the jump
    public bool isJumping = false;
    public bool jumpingNavLinkEnabled = true;
    private Vector3 jumpStartPos;
    private Vector3 jumpEndPos;
    private float jumpDuration = 1.0f; // Time to complete the jump
    private float jumpProgress = 0.0f;
    public float pathTimer = 0.0f;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updatePosition = false;  // Disable automatic movement
        _navMeshAgent.updateRotation = false;  // Disable automatic rotation
    }

    public bool IsMoving()
    {
        return foundPath;
    }
    private void Update()
    {
        if (_path == null || _path.Length == 0 || _currentPathIndex >= _path.Length || !foundPath)
        {
            foundPath = false;
            enemyController.StopMovement(); // Stop movement if no path is found
            return;
        }

        if (Vector3.Distance(transform.position, _navMeshAgent.nextPosition) > 1.0f)
        {
            _navMeshAgent.Warp(transform.position);
        }

        if (_navMeshAgent.isOnOffMeshLink && !isJumping && jumpingNavLinkEnabled)
        {
            // Start the jump
            jumpStartPos = transform.position;
            jumpEndPos = _navMeshAgent.currentOffMeshLinkData.endPos;
            jumpProgress = 0.0f;
            isJumping = true;
            enemyController.Jump();
        }

        if (isJumping)
        {
            HandleJump();
        }
        else
        {
            MoveAlongPath();
        }

        // Sync NavMeshAgent's position to the Rigidbody's position
        _navMeshAgent.nextPosition = transform.position;
    }

    private void HandleJump()
    {
        jumpProgress += Time.deltaTime / jumpDuration;

        // Calculate the jump height based on a parabolic arc
        Vector3 newPos = Vector3.Lerp(jumpStartPos, jumpEndPos, jumpProgress);
        float arcHeight = Mathf.Sin(Mathf.PI * jumpProgress) * enemyController.JumpHeight;
        newPos.y += arcHeight;

        transform.position = newPos;

        if (jumpProgress >= 1.0f)
        {
            // End the jump
            isJumping = false;
            _navMeshAgent.CompleteOffMeshLink();
            _currentPathIndex++;
        }
    }

    private void MoveAlongPath()
    {
        pathTimer += Time.deltaTime;
        Vector3 direction = (_path[_currentPathIndex] - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, _path[_currentPathIndex]);

        // Move towards the current point in the path
        if (!enemyController.disableMovement)
        {
            enemyController.Move(direction, enemyController.MoveSpeed);
        }

        // If close enough to the current path point, move to the next one
        if (distance < 0.2f)
        {
            _currentPathIndex++;
        }
        else
        {
            float rayDistance = 2.0f;
            LayerMask enemyLayer = LayerMask.GetMask("Enemy");
            Vector3 pos = transform.position;
            pos.y = transform.position.y + 2.0f;
            Vector3 dir = _path[_currentPathIndex] - transform.position;
            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, rayDistance, enemyLayer))
            {
                _path = null;
                Debug.LogError("Stopped");
            }
        }
    }

    public void FindPath(Vector3 targetPos)
    {
        if (!(_navMeshAgent.isOnNavMesh && _navMeshAgent.isActiveAndEnabled))
            return;
        NavMeshPath path = new NavMeshPath();
        NavMeshHit hit;
        float distance = 5.0f;
        bool sample = NavMesh.SamplePosition(targetPos, out hit, distance, NavMesh.AllAreas);
        if (sample)
        {
            _navMeshAgent.CalculatePath(hit.position, path);

            if ((path.status == NavMeshPathStatus.PathComplete || path.status == NavMeshPathStatus.PathPartial) && path.corners.Length > 0)
            {
                _navMeshAgent.SetDestination(hit.position);
                _path = path.corners;
                _currentPathIndex = 1; // Reset the path index to start
                foundPath = true;
                pathTimer = 0.0f;
            }
            else
            {
                foundPath = false;
            }
        }
        else
        {
            foundPath = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_path != null && _path.Length > 0)
        {
            Gizmos.color = Color.green;

            // Draw lines connecting each path corner
            for (int i = 0; i < _path.Length - 1; i++)
            {
                Gizmos.DrawLine(_path[i], _path[i + 1]);
            }

            // Optionally, draw spheres at each corner point for better visualization
            Gizmos.color = Color.red;
            for (int i = 0; i < _path.Length; i++)
            {
                Gizmos.DrawSphere(_path[i], 0.2f); // Adjust the sphere size as needed
            }
        }
    }

}
