using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundFrontSensor : MonoBehaviour
{
    public bool IsGroundInFront { get; private set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            IsGroundInFront = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            IsGroundInFront = false;    
        }
    }
}
