using System;
using UnityEngine;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SearchService;

public class PlayerController2 : MonoBehaviour {

    [SerializeField] float      m_speed = 4.0f;
    [SerializeField] float      m_jumpForce = 7.5f;
    [SerializeField] float      m_rollForce = 6.0f;

    public GameObject           enemies;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private PlayerSensor        m_groundSensor;
    private bool                m_grounded = false;
    private bool                m_rolling = false;
    public bool                 m_blocking = false;
    public int                  m_facingDirection = 1;
    private int                 m_currentAttack = 0;
    private float               m_timeSinceAttack = 0.0f;
    private float               m_delayToIdle = 0.0f;
    private float               m_rollDuration = 8.0f / 14.0f;
    private float               m_rollCurrentTime;
    private float               m_fallingTime = 1.2f;
    private float               m_currentFallingTime = 0f;
    private CapsuleCollider2D   m_standardCollider;
    private CircleCollider2D    m_rollingCollider;

    private SoundManager         m_soundManager; 

    private AnimateObject       m_animateObject;
    private PlayerAttackRange   _playerAttackRange;
    public float                damage;
    public int                  m_baseFallDamage = 20;

    // Use this for initialization
    void Start ()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<PlayerSensor>();
        m_standardCollider = GetComponent<CapsuleCollider2D>();
        m_rollingCollider = GetComponent<CircleCollider2D>();
        m_animateObject = GetComponent<AnimateObject>();
        _playerAttackRange = GetComponentInChildren<PlayerAttackRange>();
        m_soundManager = SoundManager.instance;
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
        if (!m_animateObject.Alive())
        {
            return;
        }
        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

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
            m_rolling = false;
            m_rollCurrentTime = 0f;
            m_rollingCollider.enabled = false;
            m_standardCollider.enabled = true;
            
            foreach (Transform enemy in enemies.transform)
            {
                var enemyCollider = enemy.GetComponent<Collider2D>();
                
                Physics2D.IgnoreCollision(m_standardCollider, enemyCollider, true);
            }
            
        }
        
        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.Sense())
        {
            m_grounded = true;
            
            Debug.Log(m_currentFallingTime);
            
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
        var inputX = Input.GetAxis("Horizontal");
        
        // -- Handle Animations --
        //Attack
        if(Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling && 
           !(m_animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") ||
            m_animator.GetCurrentAnimatorStateInfo(0).IsName("Fall")))
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
        }

        // Block
        else if (Input.GetMouseButton(1) && !m_rolling)
        {
            if (!m_blocking)
            {
                m_animator.SetTrigger("Block");
                m_animator.SetBool("IdleBlock", true);
                m_soundManager.PlayGuard();
                m_blocking = true;
            }
        }

        else if (Input.GetMouseButtonUp(1))
        {
            m_animator.SetBool("IdleBlock", false);
            m_blocking = false;
        }

        // Roll
        else if (Input.GetKeyDown("left shift") && !m_rolling && inputX != 0)
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            if(m_grounded)
                m_soundManager.PlaySound(SoundType.Tumble);
            m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
            m_rollingCollider.enabled = true;
            m_standardCollider.enabled = false;
            
            foreach (Transform enemy in enemies.transform)
            {
                var enemyCollider = enemy.GetComponent<Collider2D>();
                
                Physics2D.IgnoreCollision(m_rollingCollider, enemyCollider, true);
            }
        }
        
        //Jump
        else if ((Input.GetKeyDown("space") || Input.GetKeyDown("w")) && m_grounded && !m_rolling)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            // m_groundSensor.Disable(0.2f);
            
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
            if(m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);
        }
        
        // Swap direction of sprite depending on walk direction
        if (inputX > 0 && !m_blocking)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }
            
        else if (inputX < 0 && !m_blocking)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        // Move
        var legalAnimation = m_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") ||
                             m_animator.GetCurrentAnimatorStateInfo(0).IsName("Run") ||
                             m_animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") ||
                             m_animator.GetCurrentAnimatorStateInfo(0).IsName("Fall");
        if (!m_rolling && legalAnimation)
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);
        
    }

    public void PlayGrunt()
    {
        m_soundManager.PlaySound(SoundType.Grunt);
    }

    public void PlayDeath()
    {
        m_soundManager.PlayDeath();
    }

    private IEnumerator _attackEnemy(GameObject enemy)
    {
        yield return new WaitForSeconds(0.2f);
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
}
