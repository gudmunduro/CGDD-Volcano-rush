using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Overheating : MonoBehaviour
{

    private AnimateObject _animateObject;

    public float overheat;
    public float heatAmount;
    public float coolAmount;
    public bool heatResistant = false;
    private float _lastHitTime = 0f;

    public bool incoolzone;
    private Color barColor;

    public StatusBar statusBar;

    // Start is called before the first frame update
    void Start()
    {
        incoolzone = false;
        _animateObject = GetComponent<AnimateObject>();
        barColor = statusBar.transform.GetChild(0).GetComponent<Image>().color;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (heatResistant)
        {
            statusBar.transform.GetChild(0).GetComponent<Image>().color = new Color(0.57f, 0.54f, 045f);
        }
        else
        {
            statusBar.transform.GetChild(0).GetComponent<Image>().color = barColor;
        }

        if (overheat < 100 && !incoolzone)
        {
            if (!heatResistant)
                overheat += heatAmount;
            
            statusBar.Set(overheat);
        }
        else if (incoolzone)
        {
            if (overheat > 0)
            {
                overheat -= coolAmount;
                statusBar.Set(overheat);
            }
        }
        else if (!heatResistant)
        {
            if (Time.time - _lastHitTime > 1)
            {
                _animateObject.OverheatingDamage();
                _lastHitTime = Time.time;
            }
        }
    }
}
