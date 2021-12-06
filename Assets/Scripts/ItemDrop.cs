using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    public float healAmount;
    public Sprite[] foods;
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = foods[(int)Random.Range(0, foods.Length - 1)];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            gameObject.GetComponent<AnimateObject>().Heal(healAmount);
            Destroy(gameObject);
        }
    }
}
