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
    // Start is called before the first frame update
    void Start()
    {
        int _random = (int) Random.Range(0, powerups.Length - 1);
        _spriteRenderer.sprite = powerups[_random];
        _powerup = GetType(_spriteRenderer.name);
        _remainingTime = duration;
    }

    // Update is called once per frame
    void Update()
    {
        _remainingTime -= 0.1f;
        if (_remainingTime <= 0)
        {
            // TODO: clean up effects and destroy object
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            PowerType _type = GetType(_spriteRenderer.name);
            switch(_type)
            {
                case PowerType.AttackSpeed:
                    UpgradeAttackSpeed(other.gameObject);
                    break;
                case PowerType.Speed:
                    UpgradeSpeed(other.gameObject);
                    break;
            }
        }
    }

    private void UpgradeAttackSpeed(GameObject player)
    {
        // DO UPGRADES
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
