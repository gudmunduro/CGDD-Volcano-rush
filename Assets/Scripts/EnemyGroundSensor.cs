using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundSensor : MonoBehaviour
{
    public GameObject Ground { get; private set; }

    public bool IsTouchingGround => Ground != null;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            Ground = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            Ground = null;
        }
    }
}
