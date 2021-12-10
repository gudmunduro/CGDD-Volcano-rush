using System;
using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public GameObject explodeParticleEmitter;

    private SoundManager _soundManager;
    private Rigidbody2D _rigidbody;
    private Transform _arrowEnd;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _soundManager = SoundManager.instance;
    }

    private void Start()
    {
        _arrowEnd = transform.Find("ArrowEnd");
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
            var playerDistance = Vector2.Distance(GameManager.instance.player.transform.position, transform.position);
            if (playerDistance < 10)
                SoundManager.instance.PlaySound(SoundType.ArrowImpact);

            _createDestroyParticles();
        }

        Destroy(gameObject);
    }

    private void _createDestroyParticles()
    {
        var arrowEndPos = _arrowEnd.position;
        Instantiate(explodeParticleEmitter, new Vector3(arrowEndPos.x, arrowEndPos.y, -0.1f), Quaternion.identity);
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