using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    
    public Transform enemyPrefab;
    public int enemyCount;
    public Transform spawnPoint;
    public bool enemiesSpawned = false;
    public EnemyDifficultyLevel difficultyLevel = EnemyDifficultyLevel.Normal;
    private GlobalEnemyController _globalEnemyController;
    private float DistanceToPlayer => Vector2.Distance(transform.position, GameManager.instance.player.transform.position);

    private void Awake()
    {
        _globalEnemyController = spawnPoint.GetComponent<GlobalEnemyController>();
    }

    void Update()
    {
        if (DistanceToPlayer < 20.0f && !enemiesSpawned)
        {
            for (var i = 0; i < enemyCount; i++)
            {
                var enemy = Instantiate(enemyPrefab, transform.position + new Vector3(0, 1f, 0), Quaternion.identity, spawnPoint);
                var enemyController = enemy.GetComponent<EnemyController>(); 
                enemyController.globalEnemyController = _globalEnemyController;
                enemyController.EnemyDifficultyLevel = difficultyLevel;
            }

            enemiesSpawned = true;
        }
    }
}
