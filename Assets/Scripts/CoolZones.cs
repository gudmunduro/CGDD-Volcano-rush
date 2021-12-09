using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CoolZones : MonoBehaviour
{
    
    private PlayerController2 _playercontroller;
    private Light2D _light;
    private float _fadeSpeed = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        _playercontroller = GameManager.instance.player.GetComponent<PlayerController2>();
        _light = transform.GetChild(0).GetComponent<Light2D>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "CoolZoneSensor")
        {
            FindObjectOfType<Overheating>().incoolzone = true;

            if (transform.position.y < _playercontroller.playerYposition)
            {
                _playercontroller.ChangePosition(transform.position.x, transform.position.y);


                _light.intensity = 3;

            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "CoolZoneSensor")
        {
            FindObjectOfType<Overheating>().incoolzone = false;
        }
    }

    private void Update()
    {
        if (_light.intensity > 1)
        {
            _light.intensity -= _fadeSpeed * Time.deltaTime;
        }
        else
        {
            _light.intensity = 1;
        }
    }
}
