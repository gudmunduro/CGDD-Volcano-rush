using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce;
    public float moveSpeed;
    public float initialSpeed;
    
    private PlayerSensor _groundSensor;
    private Rigidbody2D _rigidbody;
    private Vector2 _velocity;
    private int _move;

    private void Start()
    {
        
        _groundSensor = transform.Find("GroundSensor").GetComponent<PlayerSensor>();
        _rigidbody = GetComponent<Rigidbody2D>();
        
        moveSpeed /= 100;
        initialSpeed /= 100;
    }
    

    void Update()
    {
        _move = 0;
        if (Input.GetKey(KeyCode.D))
        {
            _move = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _move = -1;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_groundSensor.Sense())
            {
                _rigidbody.AddForce(new Vector2(0, jumpForce));
            }
        }
    }

    private void FixedUpdate()
    {
        /* Movement */
        _velocity *= 0.80f;
        
        _velocity.x += initialSpeed * _move;

        if (_velocity.magnitude > moveSpeed)
        {
            _velocity = _velocity.normalized * moveSpeed;
        }
        
        _rigidbody.position += _velocity;
        /* Movement */
    }
}
