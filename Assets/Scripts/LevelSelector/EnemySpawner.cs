using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    //Game Object that will be instantiated when level is spawned
    [Header("Enemy Spawn Variables")]
    public GameObject Enemy; //Currently is placeholder
    public List<GameObject> EnemySpawnPoints;
    //A list of spawn points to spawn enemies

    //int variables to set number of spawnpoints for level
    [Header("Number Of Enemies Spawned")]
    [SerializeField]
    private int NumberOfEnemies = 0;
    //private int tempSpawnedPoint = -1;

    [Header("Setting number of enemies for each level")]
    public int NumberOfEnemiesLevel1 = 3;
    public int NumberOfEnemiesLevel2 = 4;
    public int NumberOfEnemiesLevel3 = 5;


    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemies(LevelSelector.currentLevel);
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
                NumberOfEnemies = NumberOfEnemiesLevel1;
                SpawnEnemiesByRandom();
                break;
            case 2:
                NumberOfEnemies = NumberOfEnemiesLevel2;
                SpawnEnemiesByRandom();
                break;
            case 3:
                NumberOfEnemies = NumberOfEnemiesLevel3;
                SpawnEnemiesByRandom();
                break;
        }
        
    }

    void SpawnEnemiesByRandom()
    {
        List<int> usedSpawnPoints = new List<int>();
        for (int i = 0; i < NumberOfEnemies; ++i)
        {
            int randomSpawnPoint = Random.Range(1, EnemySpawnPoints.Count);
            while (usedSpawnPoints.Contains(randomSpawnPoint))
            {
                randomSpawnPoint = Random.Range(0, EnemySpawnPoints.Count);
            }

            // Mark this spawn point as used
            usedSpawnPoints.Add(randomSpawnPoint);

            //tempSpawnedPoint = randomSpawnPoint;

            Vector3 positionOfSpawnPoint = EnemySpawnPoints[randomSpawnPoint].transform.position;
            Instantiate(Enemy, positionOfSpawnPoint, Quaternion.identity);  // Spawn enemy
        }
        //tempSpawnedPoint = -1;
    }
}
