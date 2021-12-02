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

    void Update()
    {  
        // Flip collider since player is flipped by sprite render, could be more clean
        float _temp = transform.localPosition.x;
        if (GetComponentInParent<PlayerController2>().m_facingDirection < transform.localPosition.x && transform.localPosition.x > 0)
            _temp *= -1;
        else if(GetComponentInParent<PlayerController2>().m_facingDirection > transform.localPosition.x && transform.localPosition.x < 0)
            _temp *= -1;
        transform.localPosition = new Vector2(_temp, transform.localPosition.y);
    }
}
