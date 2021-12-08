using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum ShootDirection
{
    Left,
    Right,
    Up,
    Down
}

public class ArrowShooter : MonoBehaviour
{
    public float initialDelay = 0;
    public float arrowFiringRate = 4.0f;
    public float arrowSpeed = 3.5f;
    public float arrowRespawnTimer = 0.2f;
    public Transform arrowPrefab;
    public ShootDirection shootDirection = ShootDirection.Left;

    private GameObject _loadedArrow;
    private float _lastArrowShotTime;
    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    void Start()
    {
        if (initialDelay > 0)
        {
            _lastArrowShotTime = Time.realtimeSinceStartup + initialDelay;   
        }
        transform.eulerAngles = _shooterRotation();
        
        _createArrow();
    }

    void Update()
    {
        if (_lastArrowShotTime + arrowFiringRate < Time.realtimeSinceStartup)
        {
            StartCoroutine(_shootArrow());
            _lastArrowShotTime = Time.realtimeSinceStartup;
        }
    }

    private IEnumerator _shootArrow()
    {
        if (_loadedArrow == null) _createArrow();
        
        var rigidBody = _loadedArrow.GetComponent<Rigidbody2D>();
        rigidBody.velocity = shootDirection switch
        {
            ShootDirection.Right => new Vector2(arrowSpeed, 0),
            ShootDirection.Left => new Vector2(-arrowSpeed, 0),
            ShootDirection.Up => new Vector2(0, arrowSpeed),
            ShootDirection.Down => new Vector2(0, -arrowSpeed)
        };
        
        yield return new WaitForSeconds(0.2f);
        
        _createArrow();
    }

    private void _createArrow()
    {
        var arrow = Instantiate(arrowPrefab);
        arrow.position = transform.position + (Vector3) _arrowSpawnOffset();
        arrow.rotation = Quaternion.Euler(new Vector3(0, 0, _arrowRotation()));
        Physics2D.IgnoreCollision(arrow.GetComponent<Collider2D>(), _collider);
        
        _loadedArrow = arrow.gameObject;
    }

    private float _arrowRotation() => shootDirection switch
    {
        ShootDirection.Right => 0,
        ShootDirection.Left => 180.0f,
        ShootDirection.Up => 90.0f,
        ShootDirection.Down => 270.0f
    };

    private Vector3 _shooterRotation() => shootDirection switch
    {
        ShootDirection.Right => Vector3.zero,
        ShootDirection.Left => new Vector3(0, 180.0f, 0),
        ShootDirection.Up => new Vector3(0, 0, 90.0f),
        ShootDirection.Down => new Vector3(0, 0, 270.0f)
    };

    private Vector2 _arrowSpawnOffset() => shootDirection switch
    {
        ShootDirection.Right => new Vector2(0, 0.3f),
        ShootDirection.Left => new Vector2(0, 0.3f),
        ShootDirection.Up => new Vector2(-0.3f, 0),
        ShootDirection.Down => new Vector2(0.3f, 0)
    };
}