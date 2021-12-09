using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemDrop : MonoBehaviour
{
    public float healAmount;
    public GameObject particles;
    public Sprite[] foods;
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        int _random = (int)Random.Range(0, foods.Length - 1);
        _spriteRenderer.sprite = foods[_random];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player" && other.GetComponent<AnimateObject>().health < 100)
        {
            SoundManager.instance.PlaySound(SoundType.Eat);
            other.GetComponent<AnimateObject>().Heal(healAmount);
            Destroy(gameObject.transform.parent.gameObject);
        }
    }

    private void OnDestroy()
    {
        Instantiate(particles, transform.position, transform.rotation);
    }
}
