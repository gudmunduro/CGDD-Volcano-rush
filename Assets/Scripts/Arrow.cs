using System;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private SoundManager _soundManager;
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _soundManager = SoundManager.instance;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var playerController = other.gameObject.GetComponent<PlayerController2>();
            var playerAnimateObject = other.gameObject.GetComponent<AnimateObject>();
            var direction = _currentDirection();

            if (direction != null && playerController.IsBlocking(direction.Value))
            {
                playerController.PlayBlockAnimation();
                _soundManager.PlaySound(SoundType.Hit);
            }
            else
            {
                playerAnimateObject.DamagePlayerHealth(20.0f);
            }
        }
        else 
        {
            float playerDistance = Vector2.Distance(GameObject.Find("Player").transform.position, transform.position);
            if (playerDistance < 10)
                SoundManager.instance.PlaySound(SoundType.ArrowImpact);
        }
        Destroy(gameObject);
    }

    private Direction? _currentDirection()
    {
        var velocity = _rigidbody.velocity;

        if (velocity.x > 0)
        {
            return Direction.Right;
        }
        if (velocity.x < 0)
        {
            return Direction.Left;
        }

        return null;
    }
}