using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject startScreen;
    public GameObject youDiedScreen;
    public GameObject youWinScreen;
    
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

    public void YouDied()
    {
        youDiedScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void YouWin()
    {
        youWinScreen.SetActive(true);
        Time.timeScale = 0;
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }
    
}
