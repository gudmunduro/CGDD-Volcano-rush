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
	private SpriteRenderer _renderer;

	private float _fadeRate = 1f;
	
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
			_renderer = GetComponent<SpriteRenderer>();
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

	    if (player && _renderer.color.b < 1)
	    {
		    var color = _renderer.color;
		    
		    color.b += _fadeRate * Time.deltaTime;
		    color.g += _fadeRate * Time.deltaTime;

		    if (color.b > 1)
		    {
			    color.b = 1;
			    color.g = 1;
		    }
		    
		    _renderer.color = color;
	    }
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

	IEnumerator PlayerDied()
	{
		yield return new WaitForSeconds(2f);
		if (!Alive() && dead)
			GameManager.instance.YouDied();
	}

	private void PlayerDeath()
	{
		dead = true;
		_animator.SetBool("noBlood", false);
		_animator.SetBool("WallSlide", false);
		_animator.SetTrigger("Death");
		SoundManager.instance.PlayDeath();
		StartCoroutine(PlayerDied());
	}

	public void DamagePlayerHealth(float damage, bool overheating = false)
	{
		health -= damage;

		if (Alive())
		{
			if (!overheating)
				_animator.SetTrigger("Hurt");

			else
			{
				var color = _renderer.color;

				color.r = 1;
				color.g = 0.2f;
				color.b = 0.2f;

				_renderer.color = color;
			}
			
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

		DamagePlayerHealth(5, true);

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
