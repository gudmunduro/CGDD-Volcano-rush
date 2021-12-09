using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class ArrowAnimator : MonoBehaviour
{
    private SpriteRenderer _arrowSprite;
    private Light2D _light;
    private float _fadeSpeed = 0.7f;
    
    void Start()
    {
        _arrowSprite = GetComponent<SpriteRenderer>();
        _light = transform.GetChild(0).GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        var color = _arrowSprite.color;

        if (GameManager.instance.keyIsPressed)
        {
            color.a = 1;
            _light.intensity = 1;
        }
        else
        {
            color.a -= _fadeSpeed * Time.deltaTime;
            _light.intensity -= _fadeSpeed * Time.deltaTime;
        }

        if (color.a <= 0)
        {
            color.a = 0;
            _light.intensity = 0;
        }
        
        _arrowSprite.color = color;
    }
}
