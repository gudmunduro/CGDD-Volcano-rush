using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
	float damage;
    // Start is called before the first frame update
    void Start()
    {
       damage = GetComponent<Weapon>().damage;
    }

	void OnTriggerEnter2D(Collider2D other)
	{
		other.GetComponent<AnimateObject>().Attack(damage);
	}

    // Update is called once per frame
    void Update()
    {
		if(Input.GetMouseButton(0))
		{
			transform.localScale = new Vector2(2, 1);
			transform.position = new Vector2(-0.5f, 0);
		}
		else if(Input.GetMouseButton(1))
		{
			transform.localScale = new Vector2(2, 1);
			transform.position = new Vector2(0.5f, 0);
		}
		else
		{
			transform.localScale = new Vector2(1, 1);
			transform.position = new Vector2(0, 0);
		}
    }

}
