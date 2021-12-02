using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolZones : MonoBehaviour
{

    private Overheating _overheating;
    // Start is called before the first frame update
    void Start()
    {
        _overheating = GetComponent<Overheating>();
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "CoolZoneSensor")
        {
            FindObjectOfType<Overheating>().incoolzone = true;
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
