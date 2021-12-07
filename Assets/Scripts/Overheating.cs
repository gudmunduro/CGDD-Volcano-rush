using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overheating : MonoBehaviour
{

    private AnimateObject _animateObject;

    public float overheat;
    public float heatAmount;
    public float coolAmount;
    public bool heatResistant = false;

    public bool incoolzone;

    public StatusBar statusBar;

    // Start is called before the first frame update
    void Start()
    {
        incoolzone = false;
        _animateObject = GetComponent<AnimateObject>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
            _animateObject.OverheatingDamage();
        }
    }
}
