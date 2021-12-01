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
    Attacking,
    Dying,
}

internal enum EnemyAnimationState
{
    Idle = 0,
    Walk = 1,
    Attack = 2,
    Die = 3
}

public class EnemyController : MonoBehaviour
{
    public float moveSpeed;
    public float runMultiplier;
    public float damage = 2;
    public float attackRate = 2f;

    private GameObject _ground;
    private EnemyVisionColliderController _enemyVisionColliderController;
    private EnemyGroundSensor _groundSensor;
    private EnemyAttackRange _enemyAttackRange;
    private GroundFrontSensor _groundFrontSensor;
    private float _platformStartX = float.NegativeInfinity;
    private float _platformEndX = float.PositiveInfinity;
    private EnemyState _enemyState;
    private int _move = 0;
    private Animator _animator;
    private Collider2D _collider;
    private float _currentTimeAttack;
    private static readonly int AnimatorStateKey = Animator.StringToHash("State");
    private static readonly int AttackAnimId = Animator.StringToHash("Attack");

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
        _groundFrontSensor = GetComponentInChildren<GroundFrontSensor>();
    }

    private void Update()
    {
        _move = 0;

        if (!GetComponent<AnimateObject>().Alive())
        {
            _enemyState = EnemyState.Dying;
            _setAnimationState(EnemyAnimationState.Die);
        }

        // Setup for default platform (if the enemy is not falling)
        if (_ground == null)
        {
            if (!_groundSensor.IsTouchingGround)
            {
                return;
            }

            _ground = _groundSensor.Ground;
            
            _startDefaultWalk();
        }

        // If the enemy has ended on another platform, configure it for that platform instead
        if (_groundSensor.IsTouchingGround && _groundSensor.Ground.name != _ground.name)
        {
            _platformStartX = float.NegativeInfinity;
            _platformEndX = float.PositiveInfinity;
        }

        // Attack player if it is in vision
        if (_enemyVisionColliderController.IsPlayerInVision)
        {
            _enemyState = EnemyState.Attacking;
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

                _attackPlayerUpdate();
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

        _currentTimeAttack += Time.deltaTime;
    }

    private void _patrolUpdate(Direction direction)
    {
        if (direction == Direction.Left && !float.IsPositiveInfinity(_platformStartX) && DistanceToPlatformLeft < 1.0f ||
            direction == Direction.Right && !float.IsNegativeInfinity(_platformEndX) && DistanceToPlatformRight < 1.0f)
        {
            _switchPatrolDirection(direction);
        }

        if (!_groundFrontSensor.IsGroundInFront || _enemyAttackRange.IsWallInAttackRange)
        {
            Debug.Log("Found end");
            switch (direction)
            {
                case Direction.Left when float.IsNegativeInfinity(_platformStartX):
                    _platformStartX = transform.position.x;
                    break;
                case Direction.Right when float.IsPositiveInfinity(_platformEndX):
                    _platformEndX = transform.position.x;
                    break;
            }
        }

        // Switch to the direction that makes more sense if enemies are colliding with each other
        if (_enemyAttackRange.AreEnemiesInAttackRange && _enemyAttackRange.EnemiesInAttackRange
            .Any(e => e.GetComponent<EnemyController>().CurrentWalkingDirection != CurrentWalkingDirection))
        {
            _startDefaultWalk();
        }

        _move = 1;
    }

    private void _switchPatrolDirection(Direction currentDirection)
    {
        _enemyState = currentDirection == Direction.Left ? EnemyState.PatrolRight : EnemyState.PatrolLeft;
        _setAnimationState(EnemyAnimationState.Walk);
        _setEnemyDirection(_getDirectionForState(_enemyState));
    }


    private IEnumerator _attackPlayer(GameObject player)
    {
        yield return new WaitForSeconds(0.35f);
        player.GetComponent<AnimateObject>().Attack(damage);
    }

    private void _attackPlayerUpdate()
    {
        // Attack player
        if (_enemyAttackRange.IsPlayerInAttackRange)
        {
            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("EnemyAttack") &&
                _currentTimeAttack >= attackRate)
            {
                _setAnimationState(EnemyAnimationState.Idle);
                _animator.SetTrigger(AttackAnimId);

                StartCoroutine(_attackPlayer(_enemyAttackRange.PlayerInAttackRange));

                _currentTimeAttack = 0;
            }
        }
        // Follow player
        else
        {
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