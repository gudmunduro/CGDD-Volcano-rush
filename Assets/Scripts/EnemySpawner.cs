using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject player;
    public Transform enemyPrefab;
    public int enemyCount;
    private bool _enemiesSpawned = false;

    private float DistanceToPlayer => Vector2.Distance(transform.position, player.transform.position);

    void Update()
    {
        if (DistanceToPlayer < 10.0f && !_enemiesSpawned)
        {
            for (var i = 0; i < enemyCount; i++)
            {
                Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            }

            _enemiesSpawned = true;
        }
    }
}
