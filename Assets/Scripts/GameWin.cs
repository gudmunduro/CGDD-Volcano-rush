using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWin : MonoBehaviour
{

    private void CallWin()
    {
        GameManager.instance.YouWin();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "Player")
        {
            Invoke(nameof(CallWin), 1);
        }
    }
}
