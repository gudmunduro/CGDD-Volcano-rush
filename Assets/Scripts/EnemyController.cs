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

internal enum EnemyAttackState
{
    Following,
    Attacking
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

public enum EnemyDifficultyLevel
{
    Easy,
    Normal,
    Hard
}

public class EnemyController : MonoBehaviour
{
    public int maxEnemiesAttackingPlayer = 2;
    public float moveSpeed;
    public float runMultiplier;
    public float damage = 2;
    public float attackRate = 1f;
    public Transform itemDropPrefab;
    public Transform upgradeDropPrefab;
    public GlobalEnemyController globalEnemyController;
    public GameObject enemyVisualisor;

    private GameObject items;
    private GameObject _ground;
    private EnemyVision _enemyVision;
    private EnemyGroundSensor _groundSensor;
    private EnemyAttackRange _enemyAttackRange;
    private GroundFrontSensor _groundFrontSensor;
    private WallSensor _wallSensor;
    private EnemyDifficultyLevel _enemyDifficultyLevelValue = EnemyDifficultyLevel.Normal;
    private float _platformStartX = float.NegativeInfinity;
    private float _platformEndX = float.PositiveInfinity;
    private EnemyState _enemyStateValue;
    private EnemyAttackState _enemyAttackState;
    private int _move = 0;
    private Animator _animator;
    private Collider2D _collider;
    private float _currentTimeAttack;
    private float _lastDirectionSwitchTime;
    private float _touchGroundTime;
    private AnimateObject _enemyAnimateObject;
    private AnimateObject _playerAnimateObject;
    private EnemyAnimationState _currentAnimationState;
    private SoundManager soundManager;
    private bool isQuitting;
    private EnemiesFollowingRange _enemiesFollowingRange;

    private static readonly int AnimatorStateKey = Animator.StringToHash("State");
    private static readonly int AttackWindupAnimTrigger = Animator.StringToHash("AttackWindup");
    private static readonly int AttackAnimTrigger = Animator.StringToHash("Attack");
    private static readonly int WalkAnimTrigger = Animator.StringToHash("Walk");
    private static readonly int IdleAnimTrigger = Animator.StringToHash("Idle");

    private EnemyState _enemyState
    {
        get => _enemyStateValue;
        set
        {
            if (_enemyState != EnemyState.Dying)
            {
                _enemyStateValue = value;
            }
        }
    }

    public EnemyDifficultyLevel EnemyDifficultyLevel
    {
        get => _enemyDifficultyLevelValue;
        set
        {
            _enemyDifficultyLevelValue = value;
            
            _enemyAnimateObject.maxHealth *= _healthMultiplierForDifficultyLevel();
            _enemyAnimateObject.health *= _healthMultiplierForDifficultyLevel();
        }
    }

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

    private bool IsPlayerJumping => !GameManager.instance.player.GetComponent<PlayerController2>().IsTouchingGround;


    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _collider = GetComponent<Collider2D>();
        _enemyAnimateObject = GetComponent<AnimateObject>();

        moveSpeed /= 100;
        _enemyState = EnemyState.Idle;
        _enemyAttackState = EnemyAttackState.Following;
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void OnDestroy()
    {
        if (_enemyState == EnemyState.Attacking && _enemyAttackState == EnemyAttackState.Attacking)
        {
            globalEnemyController.enemiesAttackingPlayer -= 1;
        }

        if (!isQuitting)
        {
            float dropThreshold = GameObject.Find("Player").GetComponent<AnimateObject>().HealthPct();
            GameObject.Find("Player").GetComponentInChildren<PlayerAttackRange>().KillRemove(gameObject);
            Vector3 enemyOffset = new Vector3(0, 1f, 0);
            // Any drop
            if (UnityEngine.Random.Range(0f, 1f) > dropThreshold)
            {
                // Health drop
                if (UnityEngine.Random.Range(0f, 1f) > .2f)
                    Instantiate(itemDropPrefab, transform.position - enemyOffset, Quaternion.identity, items.transform);

                // Upgrade drop
                else
                    Instantiate(upgradeDropPrefab, transform.position - enemyOffset, Quaternion.identity, items.transform);
            }
        }
    }

    void Start()
    {
        _groundSensor = GetComponentInChildren<EnemyGroundSensor>();
        _enemyVision = GetComponentInChildren<EnemyVision>();
        _enemyAttackRange = GetComponentInChildren<EnemyAttackRange>();
        _groundFrontSensor = GetComponentInChildren<GroundFrontSensor>();
        _wallSensor = GetComponentInChildren<WallSensor>();
        _enemiesFollowingRange = GetComponentInChildren<EnemiesFollowingRange>();
        _playerAnimateObject = GameManager.instance.player.GetComponent<AnimateObject>();
        soundManager = SoundManager.instance;
        items = GameObject.Find("Items");
        enemyVisualisor.transform.localScale = _scaleForDifficulty();
        enemyVisualisor.transform.position += new Vector3(0, _enemyPositionOffsetForDifficulty(), 0);

        Physics2D.IgnoreCollision(GameManager.instance.player.GetComponent<CapsuleCollider2D>(), _collider, true);
    }

    private void Update()
    {
        if (_enemyState == EnemyState.Dying)
        {
            return;
        }

        _move = 0;

        if (!_enemyAnimateObject.Alive())
        {
            if (_enemyState == EnemyState.Attacking && _enemyAttackState == EnemyAttackState.Attacking)
            {
                globalEnemyController.enemiesAttackingPlayer -= 1;
            }

            _enemyState = EnemyState.Dying;
            return;
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
        if (_enemyVision.IsPlayerInVision && _isPlayerInReach())
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
                if (!_enemyVision.IsPlayerInVision && !IsPlayerJumping)
                {
                    if (_enemyAttackState == EnemyAttackState.Attacking)
                    {
                        globalEnemyController.enemiesAttackingPlayer -= 1;
                    }

                    _enemyAttackState = EnemyAttackState.Following;
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

        // Fade bar when close to player
        _enemyAnimateObject.BarVisibility(Mathf.Max(0, 5 - Vector2.Distance(_playerAnimateObject.transform.position, transform.position))/5);
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
        if ((!_groundFrontSensor.IsGroundInFront || _enemyAttackRange.IsWallInAttackRange ||
             _wallSensor.IsWallInFront) &&
            _lastDirectionSwitchTime + 0.5f < Time.realtimeSinceStartup
            && _touchGroundTime + 0.2f < Time.realtimeSinceStartup)
        {
            switch (direction)
            {
                case Direction.Left when float.IsNegativeInfinity(_platformStartX):
                    _platformStartX = transform.position.x;
                    _sendMessageToAllEnemiesInFollowingRange(EnemyMessage.FoundPlatformStart, _platformStartX);
                    break;
                case Direction.Right when float.IsPositiveInfinity(_platformEndX):
                    _platformEndX = transform.position.x;
                    _sendMessageToAllEnemiesInFollowingRange(EnemyMessage.FoundPlatformEnd, _platformEndX);
                    break;
            }

            _setPatrolDirection(direction == Direction.Left ? Direction.Right : Direction.Left);
            _lastDirectionSwitchTime = Time.realtimeSinceStartup;
            _sendMessageToAllEnemiesInFollowingRange(EnemyMessage.SetDirection, _getDirectionForState(_enemyState));
        }

        // Switch to the direction that makes more sense if enemies are colliding with each other
        if (_enemyAttackRange.AreEnemiesInAttackRange && _enemyAttackRange.EnemiesInAttackRange
            .Any(e => e != null &&
                      e.GetComponent<EnemyController>().CurrentWalkingDirection != CurrentWalkingDirection))
        {
            _setPatrolDirection(Direction.Right);
        }

        _move = direction switch
        {
            Direction.Left => -1,
            Direction.Right => 1
        };
    }

    private void _setPatrolDirection(Direction direction)
    {
        _enemyState = direction == Direction.Right ? EnemyState.PatrolRight : EnemyState.PatrolLeft;
        _setAnimationState(EnemyAnimationState.Walk);
        _setEnemyDirection(_getDirectionForState(_enemyState));
    }


    private IEnumerator _attackPlayer(GameObject player)
    {
        _animator.SetTrigger(AttackWindupAnimTrigger);

        yield return new WaitForSeconds(0.5f);

        if (_enemyState == EnemyState.Dying || !_enemyAttackRange.IsPlayerInAttackRange) yield break;
        _animator.SetTrigger(AttackAnimTrigger);

        yield return new WaitForSeconds(0.14f);

        if (!_enemyAttackRange.IsPlayerInAttackRange || _enemyState == EnemyState.Dying) yield break;
            
        soundManager.PlaySound(SoundType.Swipe);

        if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "EnemyAttack" ||
            _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "EnemyAttackWindup" ||
            _animator.GetNextAnimatorClipInfo(0).Length > 0 && (
                _animator.GetNextAnimatorClipInfo(0)[0].clip.name == "EnemyAttack" ||
                _animator.GetNextAnimatorClipInfo(0)[0].clip.name == "EnemyAttackWindup"))
        {
            if (!ValidBlock())
            {
                player.GetComponent<AnimateObject>().Attack(damage * _damageMultiplierForDifficultyLevel());
            }
            else
            {
                soundManager.PlaySound(SoundType.Hit);
                player.GetComponent<PlayerController2>().PlayBlockAnimation();
            }
        }
        
        _setAnimationState(EnemyAnimationState.Idle);
    }

    public bool ValidBlock()
    {
        if (_enemyAttackRange.IsPlayerInAttackRange)
            return _enemyAttackRange.PlayerInAttackRange.GetComponent<PlayerController2>()
                .IsBlocking(_getDirectionPlayerIsIn());
        return false;
    }

    private void _attackPlayerUpdate()
    {
        switch (_enemyAttackState)
        {
            case EnemyAttackState.Following:
            {
                if (_enemyAttackRange.IsPlayerInAttackRange &&
                    _isPlayerInReach() &&
                    globalEnemyController.enemiesAttackingPlayer < maxEnemiesAttackingPlayer)
                {
                    _enemyAttackState = EnemyAttackState.Attacking;
                    globalEnemyController.enemiesAttackingPlayer += 1;
                    break;
                }

                var distanceToPlayerX = Math.Abs(transform.position.x -
                                                 GameManager.instance.player.transform.position.x);

                if (IsPlayerJumping && distanceToPlayerX < 1.0f)
                {
                    _setAnimationState(EnemyAnimationState.Idle);
                    _move = 0;
                }
                else
                {
                    var playerDirection = _getDirectionPlayerIsIn(); 
                    
                    _setAnimationState(EnemyAnimationState.Walk);
                    _move = playerDirection switch
                    {
                        Direction.Left => -(int)runMultiplier,
                        Direction.Right => (int)runMultiplier
                    };
                    _setEnemyDirection(playerDirection);
                }

                break;
            }
            case EnemyAttackState.Attacking:
            {
                if (!_enemyAttackRange.IsPlayerInAttackRange)
                {
                    _enemyAttackState = EnemyAttackState.Following;
                    globalEnemyController.enemiesAttackingPlayer -= 1;
                    break;
                }

                if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("EnemyAttack") &&
                    _currentTimeAttack >= attackRate)
                {
                    StartCoroutine(_attackPlayer(_enemyAttackRange.PlayerInAttackRange));

                    _currentTimeAttack = 0;
                }

                break;
            }
        }
    }

    private void _setEnemyDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                enemyVisualisor.transform.eulerAngles = new Vector3(0, 180.0f, 0);
                break;
            case Direction.Right:
                enemyVisualisor.transform.eulerAngles = new Vector3(0, 0, 0);
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

    private void _sendMessageToAllEnemiesInFollowingRange(EnemyMessage message, object content)
    {
        foreach (var enemy in _enemiesFollowingRange.EnemiesInRange)
        {
            if (enemy == null) continue;
            
            enemy.GetComponent<EnemyController>().SendMessageToEnemy(message, content);
        }
    }

    private bool _isPlayerInReach()
    {
        var playerDirection = _getDirectionPlayerIsIn();

        var direction = playerDirection == Direction.Left
            ? (Vector2)transform.TransformDirection(Vector2.left)
            : (Vector2)transform.TransformDirection(Vector2.right);
        
        var hit = Physics2D.Raycast(transform.position - new Vector3(0, 0.8f, 0), direction);
        return hit.collider != null && (hit.collider.gameObject.CompareTag("Player") || hit.collider.CompareTag("PlayerComponent"));
    }

    private Direction _getDirectionPlayerIsIn()
    {
        return GameManager.instance.player.transform.position.x < transform.position.x
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
        if (_enemyState == EnemyState.Dying) return;
        
        var stateName = state switch
        {
            EnemyAnimationState.Walk => "Walk",
            EnemyAnimationState.Idle => "Idle"
        };
        var trigger = state switch
        {
            EnemyAnimationState.Walk => WalkAnimTrigger,
            EnemyAnimationState.Idle => IdleAnimTrigger
        };

        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            _animator.SetTrigger(trigger);
        }
    }

    private float _damageMultiplierForDifficultyLevel() => EnemyDifficultyLevel switch
    {
        EnemyDifficultyLevel.Easy => 0.6f,
        EnemyDifficultyLevel.Normal => 1.0f,
        EnemyDifficultyLevel.Hard => 2.0f
    };
    
    private float _healthMultiplierForDifficultyLevel() => EnemyDifficultyLevel switch
    {
        EnemyDifficultyLevel.Easy => 0.4f,
        EnemyDifficultyLevel.Normal => 1.0f,
        EnemyDifficultyLevel.Hard => 2.5f
    };

    private Vector3 _scaleForDifficulty() => EnemyDifficultyLevel switch
    {
        EnemyDifficultyLevel.Easy => new Vector3(1.6f, 1.6f, 1),
        EnemyDifficultyLevel.Normal => new Vector3(1.8f, 1.8f, 1),
        EnemyDifficultyLevel.Hard => new Vector3(2.4f, 2.4f, 1)
    };

    private float _enemyPositionOffsetForDifficulty() => EnemyDifficultyLevel switch
    {
        EnemyDifficultyLevel.Easy => -0.13f,
        EnemyDifficultyLevel.Normal => -0.07f,
        EnemyDifficultyLevel.Hard => 0.1f
    };
}