using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesFollowingRange : MonoBehaviour
{
    public List<GameObject> EnemiesInRange { get; private set; }

    private void Awake()
    {
        EnemiesInRange = new List<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemiesInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemiesInRange.Remove(other.gameObject);
        }
    }
}
