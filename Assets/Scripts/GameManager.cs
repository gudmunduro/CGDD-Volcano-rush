using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject youDiedScreen;
    public GameObject youWinScreen;
    public GameObject player;
    public int enemiesKilled = 0;

    private float _timer = 0;
    private TextMeshProUGUI _timeLivedText;
    private TextMeshProUGUI _enemiesKilledText;
    
    private void Awake()
    {
        instance = this;
        //Time.timeScale = 0.5f;
    }

    private void Start()
    {
        foreach (Transform child in youWinScreen.transform)
        {
            switch (child.name)
            {
                case "TimeLived":
                    _timeLivedText = child.GetComponent<TextMeshProUGUI>();
                    break;
                case "EnemiesKilled":
                    _enemiesKilledText = child.GetComponent<TextMeshProUGUI>();
                    break;
            }
        }
    }

    private void Update()
    {
        _timer += Time.deltaTime;
    }

    public void YouDied()
    {
        youDiedScreen.SetActive(true);
        Time.timeScale = 0;
    }

    private String DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        return minutes + " Minutes and " + seconds + " Seconds";

    }
    
    public void YouWin()
    {
        youWinScreen.SetActive(true);

        _enemiesKilledText.text = "Enemies Killed: " + enemiesKilled;
        _timeLivedText.text = DisplayTime(_timer);
        
        Time.timeScale = 0;
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }
    
}
