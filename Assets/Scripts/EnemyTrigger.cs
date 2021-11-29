using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    public delegate void OnTriggerEnter(Collider2D other);

    public event OnTriggerEnter onTriggerEnter;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        onTriggerEnter?.Invoke(other);
    }
}
