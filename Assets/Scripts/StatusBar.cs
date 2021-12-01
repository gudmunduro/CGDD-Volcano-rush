using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{

    public Slider slider;

    // Start is called before the first frame update
    public void SetMax(float maxhealth)
    {
        slider.maxValue = maxhealth;
        slider.value = maxhealth;
    }

    public void Set(float health)
    {
        slider.value = health;
    }
}
