using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSensor : MonoBehaviour
{
    private bool _collision;

    public bool Sense()
    {
        return _collision;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _collision = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _collision = false;
    }
}
