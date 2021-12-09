using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackRange : MonoBehaviour
{
    public List<GameObject> EnemiesInAttackRange { get; private set; }
    public bool IsEnemyInAttackRange => EnemiesInAttackRange != null;
    public bool AreEnemiesInAttackRange => EnemiesInAttackRange.Count > 0;

    private PlayerController2 _playerController;

    private BoxCollider2D leftBoxCollider;
    private BoxCollider2D rightBoxCollider;
    
    private void Awake()
    {
        EnemiesInAttackRange = new List<GameObject>();
    }

    private void Start()
    {
        _playerController = GetComponentInParent<PlayerController2>();

        foreach (var boxCollider in GetComponents<BoxCollider2D>())
        {
            if (boxCollider.offset.x < 0)
            {
                leftBoxCollider = boxCollider;
            }
            else
            {
                rightBoxCollider = boxCollider;
            }
        }

        leftBoxCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
		if (other.gameObject.CompareTag("Enemy"))
        {
            EnemiesInAttackRange.Add(other.gameObject);
        }
    }

    public void KillRemove(GameObject enemy)
    {
        EnemiesInAttackRange.Remove(enemy);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && EnemiesInAttackRange.Contains(other.gameObject))
        {
            EnemiesInAttackRange.Remove(other.gameObject);
        }
    }

    void Update()
    {  
        // Flip collider since player is flipped by sprite render, could be more clean
        if (_playerController.m_facingDirection > 0)
        {
            rightBoxCollider.enabled = true;
            leftBoxCollider.enabled = false;
        }
        
        else if (_playerController.m_facingDirection < 0)
        {
            rightBoxCollider.enabled = false;
            leftBoxCollider.enabled = true;
        }
        
    }
}
