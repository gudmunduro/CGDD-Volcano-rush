using System;
using System.Collections;
using System.Linq;
using UnityEngine;

internal enum EnemyState
{
    PatrolLeft,
    PatrolRight,
    Attacking,
    Dying,
    Idle
}

internal enum EnemyAnimationState
{
    Idle = 0,
    Walk = 1,
    Attack = 2,
    Die = 3
}

public enum EnemyMessage
{
    FoundPlatformStart,
    FoundPlatformEnd,
    SetDirection
}

public class EnemyController : MonoBehaviour
{
    public float moveSpeed;
    public float runMultiplier;
    public float damage = 2;
    public float attackRate = 2f;

    private GameObject _ground;
    private EnemyVision _enemyVision;
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
    private float _lastDirectionSwitchTime;
    private float _touchGroundTime;
    private AnimateObject _playerAnimateObject;
    private static readonly int AnimatorStateKey = Animator.StringToHash("State");
    private static readonly int AttackAnimId = Animator.StringToHash("Attack");

    public AudioSource swipePlayer;
    public AudioSource hitPlayer;
    public AudioClip[] swipeSounds;
    public AudioClip[] hitSounds;

    public Direction CurrentWalkingDirection => _enemyState switch
    {
        EnemyState.PatrolLeft => Direction.Left,
        EnemyState.PatrolRight => Direction.Right,
        EnemyState.Attacking => _getDirectionPlayerIsIn(),
        _ => Direction.Right
    };

    private float DistanceToPlatformLeft => !float.IsNegativeInfinity(_platformStartX)
        ? Math.Abs(transform.position.x - _platformStartX)
        : float.PositiveInfinity;

    private float DistanceToPlatformRight => !float.IsPositiveInfinity(_platformStartX)
        ? Math.Abs(transform.position.x - _platformEndX)
        : float.PositiveInfinity;

    private float DistanceToPlayerInVision =>
        Vector2.Distance(_enemyVision.PlayerInVision.transform.position, transform.position);

    private bool IsPlayerInVisionJumping =>
        _enemyVision.PlayerInVision.transform.position.y - transform.position.y > 0.2f;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();

        moveSpeed /= 100;
    }

    void Start()
    {
        _groundSensor = GetComponentInChildren<EnemyGroundSensor>();
        _enemyVision = GetComponentInChildren<EnemyVision>();
        _enemyAttackRange = GetComponentInChildren<EnemyAttackRange>();
        _groundFrontSensor = GetComponentInChildren<GroundFrontSensor>();
        _playerAnimateObject = GameManager.instance.player.GetComponent<AnimateObject>();
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

            _touchGroundTime = Time.realtimeSinceStartup;
            _ground = _groundSensor.Ground;

            _startDefaultWalk();
        }

        // If the enemy has ended on another platform, configure it for that platform instead
        if (_groundSensor.IsTouchingGround && _groundSensor.Ground.name != _ground.name)
        {
            _platformStartX = float.NegativeInfinity;
            _platformEndX = float.PositiveInfinity;

            _touchGroundTime = Time.realtimeSinceStartup;
            _ground = _groundSensor.Ground;
        }

        // Attack player if it is in vision
        if (_enemyVision.IsPlayerInVision)
        {
            _enemyState = EnemyState.Attacking;
        }

        // Make enemies idle if player is dead
        if (!_playerAnimateObject.Alive())
        {
            _enemyState = EnemyState.Idle;
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
                if (!_enemyVision.IsPlayerInVision)
                {
                    _startDefaultWalk();
                    break;
                }

                _attackPlayerUpdate();
                break;
            }
            case EnemyState.Idle:
            {
                _setAnimationState(EnemyAnimationState.Idle);
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

    public void SendMessageToEnemy(EnemyMessage message, object content)
    {
        switch (message)
        {
            case EnemyMessage.FoundPlatformStart:
                _platformStartX = (float)content;
                break;
            case EnemyMessage.FoundPlatformEnd:
                _platformEndX = (float)content;
                break;
            case EnemyMessage.SetDirection:
                _setPatrolDirection((Direction)content);
                _lastDirectionSwitchTime = Time.realtimeSinceStartup;
                break;
        }
    }

    private void _patrolUpdate(Direction direction)
    {
        if ((direction == Direction.Left &&
             DistanceToPlatformLeft < 1.0f ||
             direction == Direction.Right && DistanceToPlatformRight < 1.0f) &&
            _lastDirectionSwitchTime + 0.5f < Time.realtimeSinceStartup)
        {
            _setPatrolDirection(direction == Direction.Left ? Direction.Right : Direction.Left);
            _lastDirectionSwitchTime = Time.realtimeSinceStartup;

            _sendMessageToAllEnemiesInVision(EnemyMessage.SetDirection, _getDirectionForState(_enemyState));
        }

        if ((!_groundFrontSensor.IsGroundInFront ||
             _enemyAttackRange.IsWallInAttackRange) && _lastDirectionSwitchTime + 0.5f < Time.realtimeSinceStartup
                                                    && _touchGroundTime + 0.2f < Time.realtimeSinceStartup)
        {
            switch (direction)
            {
                case Direction.Left when float.IsNegativeInfinity(_platformStartX):
                    _platformStartX = transform.position.x;
                    _sendMessageToAllEnemiesInVision(EnemyMessage.FoundPlatformStart, _platformStartX);
                    break;
                case Direction.Right when float.IsPositiveInfinity(_platformEndX):
                    _platformEndX = transform.position.x;
                    _sendMessageToAllEnemiesInVision(EnemyMessage.FoundPlatformEnd, _platformEndX);
                    break;
            }
        }

        // Switch to the direction that makes more sense if enemies are colliding with each other
        if (_enemyAttackRange.AreEnemiesInAttackRange && _enemyAttackRange.EnemiesInAttackRange
            .Any(e => e != null && e.GetComponent<EnemyController>().CurrentWalkingDirection != CurrentWalkingDirection))
        {
            _setPatrolDirection(Direction.Right);
        }

        _move = 1;
    }

    private void _setPatrolDirection(Direction direction)
    {
        _enemyState = direction == Direction.Right ? EnemyState.PatrolRight : EnemyState.PatrolLeft;
        _setAnimationState(EnemyAnimationState.Walk);
        _setEnemyDirection(_getDirectionForState(_enemyState));
    }


    private IEnumerator _attackPlayer(GameObject player)
    {
        yield return new WaitForSeconds(0.35f);
        hitPlayer.clip = hitSounds[(int)UnityEngine.Random.Range(0, hitSounds.Length)];
        hitPlayer.Play();
        if (!ValidBlock())
            player.GetComponent<AnimateObject>().Attack(damage);
    }

    public bool ValidBlock()
    {
        if(_enemyAttackRange.IsPlayerInAttackRange)
            return _enemyAttackRange.PlayerInAttackRange.GetComponent<PlayerController2>().IsBlocking(_getDirectionPlayerIsIn());
        return false;
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

                swipePlayer.clip = swipeSounds[(int)UnityEngine.Random.Range(0, swipeSounds.Length)];
                swipePlayer.Play();

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
                                                 _enemyVision.PlayerInVision.transform.position.x);

                if (distanceToPlayerX < 1.0f)
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

    private void _sendMessageToAllEnemiesInVision(EnemyMessage message, object content)
    {
        foreach (var enemy in _enemyVision.EnemiesInVision)
        {
            enemy.GetComponent<EnemyController>().SendMessageToEnemy(message, content);
        }
    }

    private Direction _getDirectionPlayerIsIn()
    {
        return _enemyVision.PlayerInVision.transform.position.x < transform.position.x
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