using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var animateObject = other.gameObject.GetComponent<AnimateObject>();
        
        Debug.Log("yayay");
        
        if (other.CompareTag("Enemy"))
        {
            animateObject.DamageEnemyHealth(100000);
        }
        else if (other.CompareTag("Player"))
        {
            animateObject.DamagePlayerHealth(100000);
        }
    }
}
