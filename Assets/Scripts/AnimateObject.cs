using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateObject : MonoBehaviour
{

	public float maxHealth;
	float health;
	public bool destroyOnKill;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

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
		Color _tmp = GetComponent<SpriteRenderer>().color;
		_tmp.a = HealthPct();
		GetComponent<SpriteRenderer>().color = _tmp;
		if(destroyOnKill && !Alive())
			Destroy(gameObject);
	}
}
