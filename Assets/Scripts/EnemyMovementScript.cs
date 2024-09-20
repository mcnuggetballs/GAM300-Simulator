using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementScript : MonoBehaviour
{
    [SerializeField]
    Entity thePlayer;
    EnemyController theEnemy;
    private void Awake()
    {
        theEnemy = GetComponent<EnemyController>();
    }

    private void FixedUpdate()
    {
        if (thePlayer)
        {
            Vector3 playerPos = thePlayer.transform.position;
            playerPos.y = 0;
            Vector3 enemyPos = transform.position;
            enemyPos.y = 0;
            if (theEnemy)
                theEnemy.Move((playerPos - enemyPos).normalized, theEnemy.MoveSpeed);
        }
    }
}
