using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    //Game Object that will be instantiated when level is spawned
    public GameObject Enemy;
    //A list of spawn points to spawn enemies
    public List<GameObject> EnemySpawnPoints;
    //int variables to set number of spawnpoints for level
    public int NumberOfEnemyLevel1 = 2;
    public int NumberOfEnemyLevel2 = 3;
    //public int NumberOfEnemyLevel3 = 4;
    //public int NumberOfEnemyLevel4 = 5;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnEnemies(int level)
    {
        switch (level)
        {
            case 1:
                //Instantiate<GameObject>(Enemy);
                break;
            case 2:
                break;
            case 3:
                break;
        }
        
    }
}
