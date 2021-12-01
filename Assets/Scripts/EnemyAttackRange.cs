using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackRange : MonoBehaviour
{
    public GameObject PlayerInAttackRange { get; private set; }
    public List<GameObject> EnemiesInAttackRange { get; private set; }
    public bool IsPlayerInAttackRange => PlayerInAttackRange != null;
    public bool AreEnemiesInAttackRange => EnemiesInAttackRange.Count > 0;
    public bool IsWallInAttackRange { get; private set; }

    private void Awake()
    {
        EnemiesInAttackRange = new List<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            PlayerInAttackRange = other.gameObject;
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            EnemiesInAttackRange.Add(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Ground"))
        {
            IsWallInAttackRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            PlayerInAttackRange = null;
        }
        else if (other.gameObject.CompareTag("Enemy") && EnemiesInAttackRange.Contains(other.gameObject))
        {
            EnemiesInAttackRange.Remove(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Ground"))
        {
            IsWallInAttackRange = false;
        }
    }
}