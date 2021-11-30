using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
	float damage;
	Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
       damage = GetComponent<Weapon>().damage;
	   _animator = GetComponentInParent<Animator>();
    }

	void OnTriggerEnter2D(Collider2D other)
	{
		other.GetComponent<AnimateObject>().Attack(damage);
	}


    // Update is called once per frame
	void Update()
    {
		if(Input.GetMouseButton(0))
			_animator.Play("GarpurPlayerAttack");
    }

}
