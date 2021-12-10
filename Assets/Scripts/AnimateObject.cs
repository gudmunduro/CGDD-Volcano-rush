using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnimateObject : MonoBehaviour
{

	public float maxHealth;
	public float health;
	public bool destroyOnKill;
	private Vector3 moveposition;
	
	public bool dead;
	public bool player = false;
	private PlayerController2 _playerController;
	private EnemyController _enemyController;
	private Animator _animator;
	public StatusBar statusBar;
	
	public GameObject PopUpScore; 
	
    // Start is called before the first frame update
	private static readonly int HitTriggerId = Animator.StringToHash("Hit");

	// Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
		statusBar.SetMax(maxHealth);
		
		if (player)
		{ 
			_animator = GetComponent<Animator>();
			_playerController = GetComponent<PlayerController2>();
		}
		else
		{
			_animator = GetComponentInChildren<Animator>();
			_enemyController = GetComponent<EnemyController>();
			BarVisibility(0);
		}
		dead = false;
    }

    // Update is called once per frame
    void Update()
    {

		statusBar.Set(health);
		
	    if(!Alive() && dead && !player && !_animator.GetCurrentAnimatorStateInfo(0).IsName("EnemyDead"))
			Destroy(gameObject.transform.parent);

			
	}

	public void BarVisibility(float alpha)
	{
		Color _temp = statusBar.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color;
		_temp.a = alpha;
		statusBar.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = _temp;
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
		dead = true;
		_animator.SetBool("noBlood", false);
		_animator.SetBool("WallSlide", false);
		_animator.SetTrigger("Death");
		_playerController.PlayDeath();
		Invoke(nameof(PlayerDied), 2);
	}

	public void DamagePlayerHealth(float damage, bool playAnimation = true)
	{
		health -= damage;

		if (Alive())
		{
			if (playAnimation)
				_animator.SetTrigger("Hurt");
			
			_playerController.PlayGrunt();
		}
		else if (!Alive() && !dead)
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

	public void DamageEnemyHealth(float damage, bool Isplayer=false)
	{
		if (!Alive()) return;
		
		health -= damage;

		if (Alive())
		{
			_animator.SetTrigger(HitTriggerId);
			_enemyController.OnEnemyHit();
		}
		else if (!Alive() && !dead)
		{
			_animator.Play("EnemyDead");
			moveposition = new Vector3(1.7f,-0.2f,0);

			if (Isplayer)
			{
				var popupScore = Instantiate(PopUpScore, transform.position+moveposition, transform.rotation);
				
				GameManager.instance.enemiesKilled += 1;
				GameManager.instance.enemiesKilledPoints += _enemyController.EnemyPoints;

				popupScore.GetComponentInChildren<TextMeshProUGUI>().text = _enemyController.EnemyPoints.ToString();
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
		//health -= 0.1f;

		DamagePlayerHealth(5, false);

		if (!Alive() && !dead)
		{
			dead = true;
			PlayerDeath();
		}
	}

	public void Respawn()
	{
		dead = false;
		health = 100;
		_animator.Play("Idle");
	}
}
