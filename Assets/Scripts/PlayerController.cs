using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public new Rigidbody2D rigidbody;
    public float jumpForce;
    public float moveSpeed;
    public float initialSpeed;

    private Vector2 _velocity;
    private bool _canJump = true;
    private int _move;

    private void Start()
    {
        moveSpeed /= 100;
        initialSpeed /= 100;
    }
    
    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.name.Substring(0, 8) == "Platform")
        {
            _canJump = true;
        }
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
            if (_canJump)
            {
                rigidbody.AddForce(new Vector2(0, jumpForce));
                _canJump = false;
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

        transform.position += (Vector3)_velocity;
        /* Movement */
    }
}
