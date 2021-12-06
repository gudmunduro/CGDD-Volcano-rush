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
    private float _remainingTime;
    private PowerType _powerup;
    private bool _active;
    private Queue _valueQueue;

    // Start is called before the first frame update
    void Start()
    {
        int _random = 1; //(int) Random.Range(0, powerups.Length - 1);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = powerups[_random];
        _powerup = GetType(_spriteRenderer.name);
        _remainingTime = duration;
        _active = false;

        _valueQueue = new Queue();
    }

    // Update is called once per frame
    void Update()
    {
        if (_active)
        {
            _remainingTime -= 0.1f;
            // TODO hide sprite or move to HUD to display remaining time
        }
        if (_remainingTime <= 0)
        {
            // TODO: clean up effects and destroy object
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            PowerType _type = GetType(_spriteRenderer.sprite.name);
            Debug.Log(_spriteRenderer.sprite.name);
            switch(_type)
            {
                case PowerType.AttackSpeed:
                    UpgradeAttackSpeed(other.gameObject, true);
                    break;
                case PowerType.Speed:
                    UpgradeSpeed(other.gameObject);
                    break;
            }
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

    private void UpgradeSpeed(GameObject player)
    {
        // DO MORE UPGRADES
    }

    private PowerType GetType(string spriteName)
    {
        switch(spriteName)
        {
            case "skill_icons10":
                return PowerType.AttackSpeed;
            case "skill_icons30":
                return PowerType.AttackSpeed;
            default:
                return PowerType.DoubleJump;
        }
    }
}
