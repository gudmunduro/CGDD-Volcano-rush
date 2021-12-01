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
	public bool player = false;
	private PlayerController2 _playerController;
	
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
		_animator = GetComponent<Animator>();
		if (player)
		{ 
			_playerController = GetComponent<PlayerController2>();
		}
		dead = false;
    }

    // Update is called once per frame
    void Update()
    {
	    if(!Alive() && dead)
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
		if (player)
		{
			if (!_playerController.IsRolling())
			{
				health -= damage;
			
				Debug.Log(health);
				if (Alive())
				{
					_animator.SetTrigger("Hurt");
				}
				else
				{
					_animator.SetBool("noBlood", false);
					_animator.SetTrigger("Death");
				}	
			}
		}
		else
		{
			health -= damage;
			
			if(Alive())
				_animator.Play("EnemyHit");
			else
				_animator.Play("EnemyDead");
		}
	}
}
