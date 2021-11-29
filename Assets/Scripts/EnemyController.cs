using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

internal enum EnemyState
{
    WalkLeft,
    WalkRight,
    Attacking
}

internal enum Direction
{
    Left,
    Right
}

public class EnemyController : MonoBehaviour
{
    public float moveSpeed;
    public float runMultiplier;
    private GameObject _ground;
    private EnemyVisionColliderController _enemyVisionColliderController;
    private EnemyGroundSensor _groundSensor;
    private float _platformStartX;
    private float _platformEndX;
    private EnemyState _enemyState;
    private int _move = 0;
    private Animator _animator;
    private Collider2D _collider;
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");

    private float DistanceToPlatformLeft => Math.Abs(transform.position.x - _platformStartX);
    private float DistanceToPlatformRight => Math.Abs(transform.position.x - _platformEndX);

    private float DistanceToPlayerInVision =>
        Vector2.Distance(_enemyVisionColliderController.PlayerInVision.transform.position, transform.position);

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();

        moveSpeed /= 100;
    }

    void Start()
    {
        _groundSensor = GetComponentInChildren<EnemyGroundSensor>();
        _enemyVisionColliderController = GetComponentInChildren<EnemyVisionColliderController>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Allow enemies to walk through each other
        if (other.gameObject.CompareTag("Enemy"))
        {
            Physics2D.IgnoreCollision(other.gameObject.GetComponent<Collider2D>(), _collider);
        }
    }

    private void Update()
    {
        if (_ground == null)
        {
            if (!_groundSensor.IsTouchingGround)
            {
                return;
            }
            
            _configureForGround(_groundSensor.Ground);
            _startDefaultWalk();
        }

        if (_groundSensor.IsTouchingGround && _groundSensor.Ground.name != _ground.name)
        {
            _configureForGround(_groundSensor.Ground);
        }
        
        _move = 0;

        if (_enemyVisionColliderController.IsPlayerInVision)
        {
            _enemyState = EnemyState.Attacking;
            _animator.SetBool(IsWalking, false);
            _animator.SetBool(IsAttacking, true);
        }

        switch (_enemyState)
        {
            case EnemyState.WalkLeft:
            {
                if (DistanceToPlatformLeft < 1.0f)
                {
                    _enemyState = EnemyState.WalkRight;
                    _animator.SetBool(IsWalking, true);
                    _setEnemyDirection(_getDirectionForState(_enemyState));
                }

                _move = 1;
                break;
            }
            case EnemyState.WalkRight:
            {
                if (DistanceToPlatformRight < 1.8f)
                {
                    _enemyState = EnemyState.WalkLeft;
                    _animator.SetBool(IsWalking, true);
                    _setEnemyDirection(_getDirectionForState(_enemyState));
                }

                _move = 1;
                break;
            }
            case EnemyState.Attacking:
            {
                if (!_enemyVisionColliderController.IsPlayerInVision)
                {
                    _animator.SetBool(IsAttacking, false);
                    _startDefaultWalk();
                    break;
                }

                _attackPlayer();
                break;
            }
        }
    }

    void FixedUpdate()
    {
        if (_move != 0)
        {
            transform.Translate(_move * moveSpeed, 0, 0);
        }
    }

    private void _attackPlayer()
    {
        if (DistanceToPlayerInVision < 1)
        {
            _animator.SetBool(IsAttacking, true);
        }
        else
        {
            _animator.SetBool(IsAttacking, false);
            _animator.SetBool(IsWalking, true);
            _move = (int) runMultiplier;
            _setEnemyDirection(_getDirectionPlayerIsIn());
        }
    }

    private void _configureForGround(GameObject ground)
    {
        _ground = ground;
        
        var localScale = ground.transform.localScale;
        var localPosition = ground.transform.localPosition;

        _platformStartX = localPosition.x - localScale.x / 2;
        _platformEndX = localPosition.x + localScale.x / 2;
        
        _startDefaultWalk();
    }

    private void _setEnemyDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                transform.eulerAngles = new Vector3(0, 180.0f, 0);
                break;
            case Direction.Right:
                transform.eulerAngles = new Vector3(0, 0, 0);
                break;
        }
    }

    private void _startDefaultWalk()
    {
        _enemyState =
            DistanceToPlatformLeft > DistanceToPlatformRight
                ? EnemyState.WalkLeft
                : EnemyState.WalkRight;
        _animator.SetBool(IsWalking, true);
        _setEnemyDirection(_getDirectionForState(_enemyState));
    }

    private Direction _getDirectionPlayerIsIn()
    {
        return _enemyVisionColliderController.PlayerInVision.transform.position.x < transform.position.x
            ? Direction.Left
            : Direction.Right;
    }

    private Direction _getDirectionForState(EnemyState state)
    {
        return state switch
        {
            EnemyState.WalkLeft => Direction.Left,
            EnemyState.WalkRight => Direction.Right,
            _ => Direction.Right
        };
    }
}