using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackRange : MonoBehaviour
{
    public List<GameObject> EnemiesInAttackRange { get; private set; }
    public bool IsEnemyInAttackRange => EnemiesInAttackRange != null;
    public bool AreEnemiesInAttackRange => EnemiesInAttackRange.Count > 0;

    private void Awake()
    {
        EnemiesInAttackRange = new List<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
		if (other.gameObject.CompareTag("Enemy"))
        {
            EnemiesInAttackRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && EnemiesInAttackRange.Contains(other.gameObject))
        {
            EnemiesInAttackRange.Remove(other.gameObject);
        }
    }
}
