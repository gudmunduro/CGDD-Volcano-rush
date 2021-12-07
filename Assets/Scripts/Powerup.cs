using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerType
{
    Stun,
    AttackSpeed,
    HeatResistance,
    Health,
    Speed,
    Vampire,
    Knockback,
    DoubleJump,
    Range,
}

public class Powerup : MonoBehaviour
{
    public Sprite[] powerups;
    private SpriteRenderer _spriteRenderer;
    public float duration;
    private PowerType _powerup;
    private bool _active;
    private Queue _valueQueue;
    private GameObject _player;
    private float _startTime;

    void Start()
    {
        int _random = 1; //(int) Random.Range(0, powerups.Length - 1);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = powerups[_random];
        _powerup = GetType(_spriteRenderer.name);
        _active = false;

        _valueQueue = new Queue();
    }

    // Update is called once per frame
    void Update()
    {
        if (_active && Time.time - _startTime > duration)
        {
            UpdateAll(_powerup, _player, false);
            Destroy(gameObject);
        }
    }

    private void UpdateAll(PowerType type, GameObject player, bool update)
    {
        switch(type)
        {
            case PowerType.AttackSpeed:
                UpgradeAttackSpeed(player, update);
                break;
            case PowerType.HeatResistance:
                UpgradeHeatResistance(player, update);
                break;
            case PowerType.Speed:
                UpgradeSpeed(player, update);
                break;
            case PowerType.DoubleJump:
                UpgradeDoubleJump(player, update);
                break;
        }
    }

    private PowerType GetType(string spriteName)
    {
        switch(spriteName)
        {
            case "skill_icons10":
                return PowerType.AttackSpeed;
            case "skill_icons15":
                return PowerType.HeatResistance;
            case "skill_icons30":
                return PowerType.Speed;
            case "skill_icons48":
                return PowerType.DoubleJump;
            default:
                return PowerType.DoubleJump;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player" && !_active)
        {
            _powerup = GetType(_spriteRenderer.sprite.name);
            _player = other.gameObject;
            UpdateAll(_powerup, _player, true);
            GetComponent<BoxCollider2D>().enabled = false;
            // TODO: pin icon to canvas and have it time out using the update method, finally destroying it
            
            _startTime = Time.time;
            _active = true;
        }
    }

    private void UpgradeAttackSpeed(GameObject player, bool upgrade)
    {
        float _temp;
        if (upgrade)
        {
            _temp = player.GetComponent<Animator>().GetFloat("AttackSpeed");
            _valueQueue.Enqueue(_temp);
            player.GetComponent<Animator>().SetFloat("AttackSpeed", _temp * 2f);

            _temp = player.GetComponent<PlayerController2>().m_attackSpeed;
            _valueQueue.Enqueue(_temp);
            player.GetComponent<PlayerController2>().m_attackSpeed = _temp * 2f;
        }
        else
        {
            _temp = (float) _valueQueue.Dequeue();
            player.GetComponent<Animator>().SetFloat("AttackSpeed", _temp / 2f);

            _temp = (float) _valueQueue.Dequeue();
            player.GetComponent<PlayerController2>().m_attackSpeed = _temp / 2f;
        }
    }

    private void UpgradeHeatResistance(GameObject player, bool upgrade)
    {
        player.GetComponent<Overheating>().heatResistant = upgrade;
    }

    private void UpgradeSpeed(GameObject player, bool upgrade)
    {
        float _temp;
        if (upgrade)
        {
            _temp = player.GetComponent<PlayerController2>().m_speed;
            _valueQueue.Enqueue(_temp);
            player.GetComponent<PlayerController2>().m_speed = _temp * 1.5f;

            _temp = player.GetComponent<Animator>().GetFloat("RunSpeed");
            _valueQueue.Enqueue(_temp);
            player.GetComponent<Animator>().SetFloat("RunSpeed", _temp * 1.5f);
        }
        else
        {
            _temp = (float) _valueQueue.Dequeue();
            player.GetComponent<PlayerController2>().m_speed = _temp / 1.5f;

            _temp = (float) _valueQueue.Dequeue();
            player.GetComponent<Animator>().SetFloat("RunSpeed", _temp / 1.5f);
        }
    }

    private void UpgradeDoubleJump(GameObject player, bool upgrade)
    {
        if (upgrade)
        {
            player.GetComponent<PlayerController2>().m_doubleJumpEnabled = true;
        }
        else
        {
            player.GetComponent<PlayerController2>().m_doubleJumpEnabled = false;
        }
    }
}
