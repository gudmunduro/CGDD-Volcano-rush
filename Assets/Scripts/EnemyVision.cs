using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public List<GameObject> EnemiesInVision { get; private set; }
    public GameObject PlayerInVision { get; private set; }
    public bool IsPlayerInVision => PlayerInVision != null;

    private void Awake()
    {
        EnemiesInVision = new List<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player" && other.gameObject.GetComponent<AnimateObject>().Alive())
        {
            PlayerInVision = other.gameObject;
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            EnemiesInVision.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            PlayerInVision = null;
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            EnemiesInVision.Remove(other.gameObject);
        }
    }

    void Update()
    {
        if (IsPlayerInVision && !PlayerInVision.GetComponent<AnimateObject>().Alive())
            PlayerInVision = null;
    }
}