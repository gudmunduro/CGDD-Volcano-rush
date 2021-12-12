using System;
using UnityEngine;
using System.Collections;
using System.Timers;
using UnityEngine.InputSystem;

public class PlayerController2 : MonoBehaviour {

    public float                m_speed = 4.0f;
    [SerializeField] float      m_jumpForce = 7.5f;
    public float                m_rollForce = 6.0f;

    [SerializeField] GameObject m_slideDust;
    
    public GameObject           enemies;
    public ParticleSystem       landingParticleSystem;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private PlayerSensor        m_groundSensor;
    private PlayerSensor        m_rollingSensor;
    private PlayerSensor        m_wallSensorR1;
    private PlayerSensor        m_wallSensorR2;
    private PlayerSensor        m_wallSensorL1;
    private PlayerSensor        m_wallSensorL2;
    private PlayerSensor        m_tightSpotSensorL2;
    private PlayerSensor        m_tightSpotSensorR2;
    private PlayerSensor        m_tightSpotSensorL1;
    private PlayerSensor        m_tightSpotSensorR1;
    private bool                m_grounded = false;
    private bool                m_rolling = false;
    private bool                m_isWallSliding = false;
    public bool                 m_blocking = false;
    private bool                m_extraJump = true;
    private bool                m_jumped = true;
    public bool                 m_doubleJumpEnabled = false;
    public Powerup              m_powerUp = null;
    public int                  m_facingDirection = 1;
    private int                 m_currentAttack = 0;
    private float               m_timeSinceAttack = 0.0f;
    private float               m_delayToIdle = 0.0f;
    private float               m_rollDuration = 8.0f / 14.0f;
    private float               m_rollCurrentTime;
    private float               m_animationRollCancelTime = 8.0f / 28.0f;
    private float               m_fallingTime = 1f;
    private float               m_currentFallingTime = 0f;
    private float               m_timeInTightSpot;
    private float               m_tightSpotRollForceMultiplierTime;
    private float               m_tightSpotRollForceMultiplier = 2.0f;
    private CapsuleCollider2D   m_standardCollider;
    private CircleCollider2D    m_rollingCollider;
    private SoundManager        m_soundManager;
    public float                m_attackSpeed;
    public bool                 m_stepFrame = false;
    private bool                m_stepped = false;
    public Camera               m_camera;
    
    private AnimateObject       m_animateObject;
    private Overheating         m_overheating;
    private PlayerAttackRange   _playerAttackRange;
    public float                damage;
    public int                  m_baseFallDamage = 20;
    public float                m_maxSlidingFallSpeed = 3f;
    public PhysicsMaterial2D    m_slipperyMaterial;

    private float               m_jumpWindow = 8.0f / 54.0f;
    private float               m_currentJumpWindowTime;
    
    private PlayerControls      m_controls;
    private Vector2             m_inputStick;

    private bool                _jump;
    private bool                _roll;
    private bool                _attack;
    private bool                _mouseAttack;
    private bool                _block;
    private bool                _mouseBlock;
    private bool                _forceRoll;

    public bool IsTouchingGround => m_groundSensor.Sense();

    public float playerXposition;
    public float playerYposition;
    
    // Use this for initialization

    private void Awake()
    {
        m_controls = new PlayerControls();
        
        // Running
        m_controls.Gameplay.Walk.performed += ctx => m_inputStick = ctx.ReadValue<Vector2>();
        m_controls.Gameplay.Walk.canceled += ctx => m_inputStick = Vector2.zero;
        
        // Jumping
        m_controls.Gameplay.Jump.performed += ctx => _jump = true;
        m_controls.Gameplay.Jump.canceled += ctx => _jump = false;
        
        // Rolling
        m_controls.Gameplay.Roll.performed += ctx => _roll = true;
        m_controls.Gameplay.Roll.canceled += ctx => _roll = false;
        
        // Attack
        m_controls.Gameplay.Attack.performed += ctx => _attack = true;
        m_controls.Gameplay.Attack.canceled += ctx => _attack = false;
        
        //Attack mouse
        m_controls.Gameplay.AttackMouse.performed += ctx => _mouseAttack = true;
        m_controls.Gameplay.AttackMouse.canceled += ctx => _mouseAttack = false;
        
        // Block
        m_controls.Gameplay.Block.performed += ctx => _block = true;
        m_controls.Gameplay.Block.canceled += ctx => _block = false;
        
        // Block
        m_controls.Gameplay.MouseBlock.performed += ctx => _mouseBlock = true;
        m_controls.Gameplay.MouseBlock.canceled += ctx => _mouseBlock = false;

        m_controls.Gameplay.Pause.performed += ctx => GameManager.instance.PauseGame();

        m_controls.Gameplay.ArrowDisplay.performed += ctx => GameManager.instance.keyIsPressed = true;
        m_controls.Gameplay.ArrowDisplay.canceled += ctx => GameManager.instance.keyIsPressed = false;

    }

    private void OnEnable()
    {
        m_controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        m_controls.Gameplay.Disable();
    }


    void Start ()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<PlayerSensor>();
        m_rollingSensor = transform.Find("RollingSensor").GetComponent<PlayerSensor>();
        m_standardCollider = GetComponent<CapsuleCollider2D>();

        m_rollingCollider = GetComponent<CircleCollider2D>();
        m_animateObject = GetComponent<AnimateObject>();
        m_overheating = GetComponent<Overheating>();
        _playerAttackRange = GetComponentInChildren<PlayerAttackRange>();
        playerXposition = transform.position.x;
        playerYposition = transform.position.y;
        
        m_soundManager = SoundManager.instance;

        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<PlayerSensor>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<PlayerSensor>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<PlayerSensor>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<PlayerSensor>();
        m_tightSpotSensorL1 = transform.Find("TightSpotSensor_L1").GetComponent<PlayerSensor>();
        m_tightSpotSensorR1 = transform.Find("TightSpotSensor_R1").GetComponent<PlayerSensor>();
        m_tightSpotSensorL2 = transform.Find("TightSpotSensor_L2").GetComponent<PlayerSensor>();
        m_tightSpotSensorR2 = transform.Find("TightSpotSensor_R2").GetComponent<PlayerSensor>();
    }

    public bool IsRolling()
    {
        return m_rolling;
    }
    
    // Player and enemy facing each other yields valid block
    public bool IsBlocking(Direction enemyDirection)
    {
        if (m_blocking)
        {
            if (enemyDirection == Direction.Left && m_facingDirection == 1)
                return true;
            else if (enemyDirection == Direction.Right && m_facingDirection == -1)
                return true;
        }
        return false;
    }
    
    // Update is called once per frame
    void Update ()
    {
        if (GameManager.instance.youDiedScreen.activeSelf 
            || GameManager.instance.youWinScreen.activeSelf 
            || GameManager.instance.pauseGameScreen.activeSelf)
        {
            return;
        }
        
        if (!m_animateObject.Alive())
        {
            m_currentFallingTime = 0;
            return;
        }
        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        if (!m_grounded)
        {
            m_standardCollider.sharedMaterial = m_slipperyMaterial;
            m_currentJumpWindowTime += Time.deltaTime;
        }

        else
        {
            m_extraJump = true;
            m_standardCollider.sharedMaterial = null;
            m_currentJumpWindowTime = 0;
        }

        if (((m_tightSpotSensorL2.Sense() && m_tightSpotSensorL1.Sense()) || m_tightSpotSensorR2.Sense() && m_tightSpotSensorR1.Sense()) && !m_grounded && !m_rolling)
        {
            m_timeInTightSpot += Time.deltaTime;
        }
        else
        {
            m_timeInTightSpot = 0;
        }

        if (m_timeInTightSpot > 0.05)
        {
            _forceRoll = true;
            m_tightSpotRollForceMultiplierTime = 0.5f;
            m_timeInTightSpot = 0;
        }
        
        // Increase timer that checks roll duration
        if (m_rolling)
        {
            m_rollCurrentTime += Time.deltaTime;
        }
        
        
        if (!m_grounded && m_body2d.velocity.y < 0)
        {
            m_currentFallingTime += Time.deltaTime;
        }

        // Disable rolling if timer extends duration
        if (m_rollCurrentTime > m_rollDuration)
        {
            m_rollCurrentTime = 0f;
            
            if (m_rollingSensor.Sense())
            {
                
                m_animator.SetTrigger("ContinueRoll");
                m_soundManager.PlaySound(SoundType.Tumble);
                m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
            }
            else
            {
                m_animator.SetTrigger("StandUp");

                m_rolling = false;
                m_rollingCollider.enabled = false;
                m_standardCollider.enabled = true;
                m_tightSpotRollForceMultiplierTime = 0;

                foreach (Transform enemy in enemies.transform)
                {
                    var enemyCollider = enemy.GetComponent<Collider2D>();
                
                    Physics2D.IgnoreCollision(m_standardCollider, enemyCollider, true);
                }
            }
        }
        
        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.Sense())
        {
            if (!m_rolling && !m_grounded)
            {
                m_soundManager.PlaySound(SoundType.Step);
                landingParticleSystem.Emit(4);
            }

            m_grounded = true;
            m_jumped = false;

            if (m_currentFallingTime > m_fallingTime)
            {
                m_animateObject.DamagePlayerHealth(m_baseFallDamage * Mathf.Floor(m_currentFallingTime));
            }

            m_animator.SetBool("Grounded", m_grounded);
            m_currentFallingTime = 0f;
            
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.Sense())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }
        
        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        // -- Handle input and movement --

        var illegaAnimation = m_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") ||
                              m_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") ||
                              m_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3") ||
                              m_animator.GetCurrentAnimatorStateInfo(0).IsName("Wall Slide");
        
        if (Mathf.Abs(m_inputStick.x) < 0.7f)
        {
            m_inputStick.x = 0;
        }
        
        // Swap direction of sprite depending on walk direction
        if (m_inputStick.x > 0 && !m_blocking && !illegaAnimation)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }
            
        else if (m_inputStick.x < 0 && !m_blocking && !illegaAnimation)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }


        var illegalAnimation2 = m_animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") ||
                                m_animator.GetCurrentAnimatorStateInfo(0).IsName("Fall") ||
                                m_animator.GetCurrentAnimatorStateInfo(0).IsName("Block") ||
                                m_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle Block") ||
                                m_animator.GetCurrentAnimatorStateInfo(0).IsName("Wall Slide");
        
        // -- Handle Animations --
        
        if (m_animateObject.Alive())
        {
            m_isWallSliding = !m_grounded && ((m_wallSensorR1.Sense() && m_wallSensorR2.Sense()) || (m_wallSensorL1.Sense() && m_wallSensorL2.Sense()));
            m_animator.SetBool("WallSlide", m_isWallSliding);
            
            if (m_isWallSliding)
            {
                if (!m_soundManager.PlayingSlide() && m_animator.GetCurrentAnimatorStateInfo(0).IsName("Wall Slide"))
                    m_soundManager.PlaySlide();

                m_currentFallingTime = 0;

                var velocity = m_body2d.velocity;

                if (m_body2d.velocity.y < -m_maxSlidingFallSpeed)
                {
                    velocity.y = -m_maxSlidingFallSpeed;
                }

                if (m_wallSensorR1.Sense() && m_wallSensorR2.Sense())
                {
                    GetComponent<SpriteRenderer>().flipX = false;
                    m_facingDirection = 1;
                }
                else
                {
                    GetComponent<SpriteRenderer>().flipX = true;
                    m_facingDirection = -1;
                }

                m_body2d.velocity = velocity;
            }
            else
            {
                m_animator.SetTrigger("OutOfSlideFall");
                if (m_soundManager.PlayingSlide())
                    m_soundManager.StopSolo();
            }
        }
        if (_playerAttackRange.AreEnemiesInAttackRange && !m_soundManager.fading)
        {
            StartCoroutine(m_soundManager.FadeAgro(0.5f, 1f));
        }
        else if (!m_soundManager.fading)
        {
            StartCoroutine(m_soundManager.FadeAgro(0f, .5f));
        }
            
        
        //Attack
        if((_attack || _mouseAttack) && m_timeSinceAttack > 0.25f && 
           !illegalAnimation2 && !m_rollingSensor.Sense() && (m_rolling && m_animationRollCancelTime < m_rollCurrentTime || !m_rolling))
        {
            m_currentAttack++;
            _attackEnemyUpdate();

            // Loop back to one after third attack
            if (m_currentAttack > 3)
                m_currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            m_animator.SetTrigger("Attack" + m_currentAttack);

            // Reset timer
            m_timeSinceAttack = 0.0f;

            var check = m_camera.WorldToScreenPoint(transform.position);

            if (_mouseAttack)
            {
                if (Mouse.current.position.x.ReadValue() < check.x)
                {
                    GetComponent<SpriteRenderer>().flipX = true;
                    m_facingDirection = -1;
                }
                else
                {
                    GetComponent<SpriteRenderer>().flipX = false;
                    m_facingDirection = 1;
                }
            }

            m_body2d.velocity = new Vector2(0f, 0f);

        }

        // Block
        else if ((_block || _mouseBlock) && !(illegaAnimation || illegalAnimation2) && 
                 !m_rollingSensor.Sense() && (m_rolling && m_animationRollCancelTime < m_rollCurrentTime || !m_rolling))
        {
            if (!m_blocking)
            {
                m_animator.SetTrigger("Block");
                m_animator.SetBool("IdleBlock", true);
                m_soundManager.PlayGuard();
                m_blocking = true;
                
                var check = m_camera.WorldToScreenPoint(transform.position);

                if (_mouseBlock)
                {
                    if (Mouse.current.position.x.ReadValue() < check.x)
                    {
                        GetComponent<SpriteRenderer>().flipX = true;
                        m_facingDirection = -1;
                    }
                    else
                    {
                        GetComponent<SpriteRenderer>().flipX = false;
                        m_facingDirection = 1;
                    } 
                }

                m_body2d.velocity = new Vector2(0f, 0f);
            }
        }

        else if (!(_block || _mouseBlock) && m_blocking)
        {
            m_animator.SetBool("IdleBlock", false);
            m_blocking = false;
        }

        // Roll
        else if ((_roll || _forceRoll) && !m_rolling)
        {
            int rollDirection = 0;
            if (!m_animator.GetCurrentAnimatorStateInfo(0).IsName("Wall Slide"))
            {
                if (m_inputStick.x > 0)
                {
                    rollDirection = 1;
                }
                else if (m_inputStick.x < 0)
                {
                    rollDirection = -1;
                }
                else
                {
                    rollDirection = m_facingDirection;
                }
            }
            else
            {
                if (m_wallSensorR1.Sense() && m_wallSensorR2.Sense() && m_inputStick.x < 0)
                {
                    rollDirection = -1;
                }
                
                else if (m_wallSensorL1.Sense() && m_wallSensorL2.Sense() && m_inputStick.x > 0)
                {
                    rollDirection = 1;
                }
            }

            if (rollDirection != 0)
            {
                m_rolling = true;
                m_animator.ResetTrigger("StandUp");
                m_animator.SetTrigger("Roll");
                if(m_grounded)
                    m_soundManager.PlaySound(SoundType.Tumble);
                else
                    m_soundManager.PlaySound(SoundType.TumbleAir);

                var multiplier = 1.0f;
                if (m_tightSpotRollForceMultiplierTime > 0)
                {
                    multiplier = m_tightSpotRollForceMultiplier;

                    m_tightSpotRollForceMultiplierTime -= Time.deltaTime;
                }
                
                m_body2d.velocity = new Vector2(rollDirection * m_rollForce * multiplier, m_body2d.velocity.y);
                
                m_rollingCollider.enabled = true;
                m_standardCollider.enabled = false;

                foreach (Transform enemy in enemies.transform)
                {
                    var enemyCollider = enemy.GetComponent<Collider2D>();
                    
                    Physics2D.IgnoreCollision(m_rollingCollider, enemyCollider, true);
                }
                
                _forceRoll = false;
            }
        }
        
        // Switch directions while rolling
        else if (_roll && m_rolling && !m_animator.GetCurrentAnimatorStateInfo(0).IsName("Wall Slide"))
        {
            int rollDirection;
            if (m_inputStick.x > 0)
            {
                rollDirection = 1;
            }
            else if (m_inputStick.x < 0)
            {
                rollDirection = -1;
            }
            else
            {
                rollDirection = m_facingDirection;
            }

            if (Math.Sign(rollDirection) != Math.Sign(m_body2d.velocity.x) && Math.Sign(m_body2d.velocity.x) != 0)
            {
                m_body2d.velocity = new Vector2(-m_body2d.velocity.x, m_body2d.velocity.y);
            }
        }
        
        //Jump
        else if (_jump && ((m_grounded || (m_doubleJumpEnabled && m_extraJump)) || (m_currentJumpWindowTime < m_jumpWindow && !m_jumped)) && (m_rolling && m_animationRollCancelTime < m_rollCurrentTime || !m_rolling))
        {
            if (!m_grounded && m_currentJumpWindowTime >= m_jumpWindow)
            {
                m_extraJump = false;
            }

            m_jumped = true;
            
            m_soundManager.PlayJump(m_grounded || m_currentJumpWindowTime < m_jumpWindow);
            m_animator.SetTrigger("Jump");
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
            
        }

        //Run
        else if (Mathf.Abs(m_inputStick.x) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
            m_animator.SetFloat("RunSpeed", Mathf.Abs(m_inputStick.x));
            if ((m_stepFrame && !m_stepped) && (m_grounded && !m_rolling))
            {
                m_soundManager.PlaySound(SoundType.Step);
                m_stepped = true;
            }
            if (!m_stepFrame)
                m_stepped = false;
                
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
            if(m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);
        }
        

        // Move
        var legalAnimation = m_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") ||
                             m_animator.GetCurrentAnimatorStateInfo(0).IsName("Run") ||
                             m_animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") ||
                             m_animator.GetCurrentAnimatorStateInfo(0).IsName("Fall") ||
                             m_animator.GetCurrentAnimatorStateInfo(0).IsName("Wall Slide");
        if (!m_rolling && legalAnimation)
            m_body2d.velocity = new Vector2(m_inputStick.x * m_speed, m_body2d.velocity.y);

        _jump = false;
        _attack = false;
        _mouseAttack = false;
    }

    public void PlayGrunt()
    {
        m_soundManager.PlaySound(SoundType.Grunt);
    }

    public void PlayBlockAnimation()
    {
        m_animator.SetTrigger("Block");
    }

    private IEnumerator _attackEnemy(GameObject enemy)
    {
        yield return new WaitForSeconds(m_attackSpeed);
        var legalAnimation = m_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") ||
                             m_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") ||
                             m_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3");
        if (enemy && legalAnimation)
        {
            m_soundManager.PlaySound(SoundType.Bone);
            enemy.GetComponent<AnimateObject>().Attack(damage);
        }
    }

    private void _attackEnemyUpdate()
    {
        if (_playerAttackRange.IsEnemyInAttackRange)
        {
            if (m_currentAttack > 0)
            {
                m_soundManager.PlaySound(SoundType.Swipe);
                foreach (var enemy in _playerAttackRange.EnemiesInAttackRange)
                {
                    StartCoroutine(_attackEnemy(enemy));
                }
            }
        }
    }

    public void ChangePosition(float x , float y)
    {
        playerXposition = x;
        playerYposition = y;
    }

    public void Respawn()
    {
        m_animateObject.Respawn();
        if (m_powerUp != null)
        {
            m_powerUp.CleanUp(false);
            m_powerUp = null;
        }
        //ameObject.Find("PowerUpHUD").GetComponent<UnityEngine.UI.Image>().color = null;
        m_overheating.overheat = 0;
        gameObject.transform.position = new Vector3(playerXposition, playerYposition, 0);
        m_body2d.gravityScale = 2;
        
        m_inputStick = Vector2.zero;
        _jump = false;
        _roll = false; 
        _attack = false; 
        _mouseAttack = false; 
        _block = false;
        _mouseBlock = false;
        GameManager.instance.keyIsPressed = false;
        
    }
    
    // Animation Events
    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }
    
}
