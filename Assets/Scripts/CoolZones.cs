using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolZones : MonoBehaviour
{

    private PlayerController2 _playercontroller;
    // Start is called before the first frame update
    void Start()
    {
        _playercontroller = GameManager.instance.player.GetComponent<PlayerController2>();
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "CoolZoneSensor")
        {
            FindObjectOfType<Overheating>().incoolzone = true;
            _playercontroller.ChangePosition(transform.position.x, transform.position.y); 
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "CoolZoneSensor")
        {
            FindObjectOfType<Overheating>().incoolzone = false;
        }
    }
}
