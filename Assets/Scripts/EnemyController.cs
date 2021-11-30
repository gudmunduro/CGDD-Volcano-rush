using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

internal enum EnemyState
{
    PatrolLeft,
    PatrolRight,
    Attacking
}

internal enum EnemyAnimationState
{
    Idle = 0,
    Walk = 1,
    Attack = 2
}

public class EnemyController : MonoBehaviour
{
    public float moveSpeed;
    public float runMultiplier;

    private GameObject _ground;
    private EnemyVisionColliderController _enemyVisionColliderController;
    private EnemyGroundSensor _groundSensor;
    private EnemyAttackRange _enemyAttackRange;
    private float _platformStartX;
    private float _platformEndX;
    private EnemyState _enemyState;
    private int _move = 0;
    private Animator _animator;
    private Collider2D _collider;
    private GameObject _playerToAttack;
    private static readonly int AnimatorStateKey = Animator.StringToHash("State");

    public Direction CurrentWalkingDirection => _enemyState switch
    {
        EnemyState.PatrolLeft => Direction.Left,
        EnemyState.PatrolRight => Direction.Right,
        EnemyState.Attacking => _getDirectionPlayerIsIn()
    };

    private float DistanceToPlatformLeft => Math.Abs(transform.position.x - _platformStartX);
    private float DistanceToPlatformRight => Math.Abs(transform.position.x - _platformEndX);

    private float DistanceToPlayerInVision =>
        Vector2.Distance(_enemyVisionColliderController.PlayerInVision.transform.position, transform.position);

    private bool IsPlayerInVisionJumping =>
        _enemyVisionColliderController.PlayerInVision.transform.position.y - transform.position.y > 0.2f;


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
        _enemyAttackRange = GetComponentInChildren<EnemyAttackRange>();
    }

    private void Update()
    {
        _move = 0;

        // Setup for default platform (if the enemy is not falling)
        if (_ground == null)
        {
            if (!_groundSensor.IsTouchingGround)
            {
                return;
            }

            _configureForGround(_groundSensor.Ground);
            _startDefaultWalk();
        }

        // If the enemy has ended on another platform, configure it for that platform instead
        if (_groundSensor.IsTouchingGround && _groundSensor.Ground.name != _ground.name)
        {
            _configureForGround(_groundSensor.Ground);
        }

        // Attack player if it is in vision
        if (_enemyVisionColliderController.IsPlayerInVision)
        {
            _enemyState = EnemyState.Attacking;
            _setAnimationState(EnemyAnimationState.Attack);
        }

        switch (_enemyState)
        {
            case EnemyState.PatrolLeft:
            {
                _patrolUpdate(Direction.Left);
                break;
            }
            case EnemyState.PatrolRight:
            {
                _patrolUpdate(Direction.Right);
                break;
            }
            case EnemyState.Attacking:
            {
                if (!_enemyVisionColliderController.IsPlayerInVision)
                {
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

        if (_playerToAttack != null)
        {
            _playerToAttack.GetComponent<AnimateObject>().Attack(0.01f);
        }
    }

    private void _patrolUpdate(Direction direction)
    {
        if (direction == Direction.Left && DistanceToPlatformLeft < 1.0f ||
            direction == Direction.Right && DistanceToPlatformRight < 1.0f)
        {
            _enemyState = direction == Direction.Left ? EnemyState.PatrolRight : EnemyState.PatrolLeft;
            _setAnimationState(EnemyAnimationState.Walk);
            _setEnemyDirection(_getDirectionForState(_enemyState));
        }

        // Switch to the direction that makes more sense if enemies are colliding with each other
        if (_enemyAttackRange.AreEnemiesInAttackRange && _enemyAttackRange.EnemiesInAttackRange.Any(e =>
            e.GetComponent<EnemyController>().CurrentWalkingDirection != CurrentWalkingDirection))
        {
            _startDefaultWalk();
        }

        _move = 1;
    }

    private void _attackPlayer()
    {
        // Attack player
        if (_enemyAttackRange.IsPlayerInAttackRange)
        {
            _setAnimationState(EnemyAnimationState.Attack);
            _playerToAttack = _enemyAttackRange.PlayerInAttackRange;
        }
        // Follow player
        else
        {
            _playerToAttack = null;

            if (IsPlayerInVisionJumping)
            {
                var distanceToPlayerX = Math.Abs(transform.position.x -
                                                 _enemyVisionColliderController.PlayerInVision.transform.position.x);

                if (distanceToPlayerX < 2.0f)
                {
                    _setAnimationState(EnemyAnimationState.Idle);
                    _move = 0;
                }
            }
            else
            {
                _setAnimationState(EnemyAnimationState.Walk);
                _move = (int)runMultiplier;
                _setEnemyDirection(_getDirectionPlayerIsIn());
            }
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
                ? EnemyState.PatrolLeft
                : EnemyState.PatrolRight;
        _setAnimationState(EnemyAnimationState.Walk);
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
            EnemyState.PatrolLeft => Direction.Left,
            EnemyState.PatrolRight => Direction.Right,
            _ => Direction.Right
        };
    }

    private void _setAnimationState(EnemyAnimationState state)
    {
        _animator.SetInteger(AnimatorStateKey, (int)state);
    }
}