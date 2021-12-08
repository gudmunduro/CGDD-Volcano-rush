using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSensor : MonoBehaviour
{
    private bool _collision;
    private bool _disabled;
    private float _disabledTime;
    private float _currentDisabledTime;

    public bool Sense()
    {
        Debug.Log("Yo");
        return !_disabled && _collision;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            _collision = true;    
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            _collision = false;    
        }
    }

    public void Disable(float time)
    {
        _disabled = true;
        _currentDisabledTime = 0f;
        _disabledTime = time;
    }


    private void Update()
    {
        if (!_disabled) return;
        _currentDisabledTime += Time.deltaTime;

        if (_currentDisabledTime >= _disabledTime)
        {
            _disabled = false;
        }
    }
}
