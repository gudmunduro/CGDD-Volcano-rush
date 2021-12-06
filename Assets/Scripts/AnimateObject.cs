using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateObject : MonoBehaviour
{

	public float maxHealth;
	public float health;
	public bool destroyOnKill;
	
	public bool dead;
	public bool player = false;
	private PlayerController2 _playerController;
	private Animator _animator;
	public StatusBar statusBar;
	
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
		
		_animator = GetComponent<Animator>();
		if (player)
		{ 
			statusBar.SetMax(maxHealth);
			_playerController = GetComponent<PlayerController2>();
		}
		dead = false;
    }

    // Update is called once per frame
    void Update()
    {
	    if(!Alive() && dead && !player)
			Destroy(gameObject);

		if (player)
			statusBar.Set(health);
	}

	public float HealthPct()
	{
		return health/maxHealth;
	}

	public bool Alive()
	{
		return health > 0;
	}

	private void PlayerDied()
	{
		GameManager.instance.YouDied();
	}

	private void PlayerDeath()
	{
		_animator.SetBool("noBlood", false);
		_animator.SetTrigger("Death");
		_playerController.PlayDeath();
		Invoke(nameof(PlayerDied), 2);
	}

	public void DamagePlayerHealth(float damage)
	{
		health -= damage;

		if (Alive())
		{
			_animator.SetTrigger("Hurt");
			_playerController.PlayGrunt();
		}
		else
		{
			PlayerDeath();
		}	
	}

	public void Heal(float amount)
	{
		if (health + amount <= maxHealth)
			health += amount;
		else
			health = maxHealth;
	}

	public void DamageEnemyHealth(float damage, bool player=false)
	{
		health -= damage;

		if(Alive())
			_animator.Play("EnemyHit");
		else
		{
			_animator.Play("EnemyDead");

			if (player)
			{
				GameManager.instance.enemiesKilled++;
			}
		}
	}
	
	public void Attack(float damage)
	{
		if (player)
		{
			if (!_playerController.IsRolling())
			{
				DamagePlayerHealth(damage);
			}
		}
		else
		{
			DamageEnemyHealth(damage, true);
		}
	}

	public void OverheatingDamage()
	{
		health -= 0.1f;

		if (!Alive() && !dead)
		{
			dead = true;
			PlayerDeath();
		}
	}
}
