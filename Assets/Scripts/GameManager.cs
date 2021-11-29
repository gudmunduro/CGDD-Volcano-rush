using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject startScreen;

    private void Awake()
    {
        instance = this;
        Time.timeScale = 0;
    }

    public void StartGame()
    {
        startScreen.SetActive(false);
        Time.timeScale = 1;
    }
}
