using System;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<AnimateObject>().DamagePlayerHealth(20.0f);
        }
        
        Destroy(gameObject);
    }
}