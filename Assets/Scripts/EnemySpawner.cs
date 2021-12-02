using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    
    public Transform enemyPrefab;
    public int enemyCount;
    public Transform spawnPoint;
    private bool _enemiesSpawned = false;
    private float DistanceToPlayer => Vector2.Distance(transform.position, GameManager.instance.player.transform.position);

    void Update()
    {
        if (DistanceToPlayer < 20.0f && !_enemiesSpawned)
        {
            for (var i = 0; i < enemyCount; i++)
            {
                Instantiate(enemyPrefab, transform.position, Quaternion.identity, spawnPoint);
            }

            _enemiesSpawned = true;
        }
    }
}
