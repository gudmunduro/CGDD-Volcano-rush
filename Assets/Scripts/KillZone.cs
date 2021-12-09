using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var animateObject = other.gameObject.GetComponent<AnimateObject>();

        if (other.CompareTag("Enemy"))
        {
            animateObject.DamageEnemyHealth(100000);
        }
        else if (other.CompareTag("Player"))
        {
            animateObject.DamagePlayerHealth(100000);
            
            var body = other.GetComponent<Rigidbody2D>();

            body.gravityScale = 0.1f;
            body.velocity = new Vector2(0f, -0.5f);

        }
    }
}
