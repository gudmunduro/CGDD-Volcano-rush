using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.Universal;

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

    private GameObject _powerUpUI;
    private Image _powerUpImage;
    
    void Start()
    {
        //int _random = 3; //TESTING
        int _random = (int) Random.Range(0, powerups.Length - 1);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = powerups[_random];
        _powerup = GetType(_spriteRenderer.sprite.name);
        _active = false;

        _powerUpUI = GameObject.Find("PowerUpHUD");
        _powerUpImage = _powerUpUI.GetComponent<Image>();
        _powerUpUI.GetComponent<Animator>().SetFloat("Duration", 1f/duration);
        transform.parent.GetChild(1).GetComponent<Light2D>().color = GetColor(_powerup);
        
        _valueQueue = new Queue();
    }

    // Update is called once per frame
    void Update()
    {
        if (_active && Time.time - _startTime > duration)
        {
            CleanUp(true);
        }
    }

    public void CleanUp(bool playSound)
    {
        UpdateAll(_powerup, _player, false);
            
        _powerUpImage.sprite = null;
        _powerUpUI.GetComponent<Animator>().Play("PowerUpStart");

        if (playSound)
            SoundManager.instance.PlayPower(false);

        Destroy(transform.parent.gameObject);
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

    private Color HexToRGB(int hexColor)
    {
        float r = (float) ((hexColor & 0xff0000) >> 16);
        float g = (float) ((hexColor & 0xff00) >> 8);
        float b = (float) (hexColor & 0xff);
        return new Color(r / 255f, g / 255f, b / 255f);
    }

    private Color GetColor(PowerType type)
    {
        switch(type)
        {
            case PowerType.AttackSpeed:
                return HexToRGB(0x15983A);
            case PowerType.HeatResistance:
                return HexToRGB(0xC32C37);
            case PowerType.Speed:
                return HexToRGB(0xB55EB2);
            case PowerType.DoubleJump:
                return HexToRGB(0x0D82CE);
            default:
                return Color.white;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player" && !_active)
        {
            _player = other.gameObject;
            if (_player.GetComponent<PlayerController2>().m_powerUp == null)
            {
                _powerup = GetType(_spriteRenderer.sprite.name);
                _powerUpUI.GetComponent<Animator>().Play("PowerUpFade");
                UpdateAll(_powerup, _player, true);
                GetComponent<BoxCollider2D>().enabled = false;

                _spriteRenderer.enabled = false;
                
                _powerUpImage.sprite = _spriteRenderer.sprite;
                
                _startTime = Time.time;
                _player.GetComponent<PlayerController2>().m_powerUp = this;
                transform.parent.GetChild(1).gameObject.active = false;
                SoundManager.instance.PlayPower(true);
                _active = true;
            }            
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
            player.GetComponent<Animator>().SetFloat("AttackSpeed", _temp);

            _temp = (float) _valueQueue.Dequeue();
            player.GetComponent<PlayerController2>().m_attackSpeed = _temp;
        }
    }

    private void UpgradeHeatResistance(GameObject player, bool upgrade)
    {
        player.GetComponent<Overheating>().heatResistant = upgrade;
    }

    private void UpgradeSpeed(GameObject player, bool upgrade)
    {
        float _temp;
        PlayerController2 _pc2 = player.GetComponent<PlayerController2>();
        if (upgrade)
        {
            _temp = _pc2.m_speed;
            _valueQueue.Enqueue(_temp);
            _pc2.m_speed = _temp * 1.5f;

            _temp = _pc2.m_rollForce;
            _valueQueue.Enqueue(_temp);
            _pc2.m_rollForce = _temp * 1.4f;

            _temp = player.GetComponent<Animator>().GetFloat("RunSpeed");
            _valueQueue.Enqueue(_temp);
            player.GetComponent<Animator>().SetFloat("RunSpeed", _temp * 1.5f);
        }
        else
        {
            _temp = (float) _valueQueue.Dequeue();
            _pc2.m_speed = _temp;

            _temp = (float) _valueQueue.Dequeue();
            _pc2.m_rollForce = _temp;

            _temp = (float) _valueQueue.Dequeue();
            player.GetComponent<Animator>().SetFloat("RunSpeed", _temp);
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
