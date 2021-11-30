using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateObject : MonoBehaviour
{

	public float maxHealth;
	float health;
	public bool destroyOnKill;
	Animator _animator;
	public bool dead;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
		_animator = GetComponent<Animator>();
		dead = false;
    }

    // Update is called once per frame
    void Update()
    {
		if(dead && !Alive())
			Destroy(gameObject);
	}

	public float HealthPct()
	{
		return health/maxHealth;
	}

	public bool Alive()
	{
		return health > 0;
	}

	public void Attack(float damage)
	{
		health -= damage;
		Debug.Log(health);
		if(Alive())
			_animator.Play("GarpurEnemyHit");
		else
			_animator.Play("GarpurEnemyDead");
	}
}
