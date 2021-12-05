using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject youDiedScreen;
    public GameObject youWinScreen;
    public GameObject player;
    public TextMeshProUGUI liveTimerText;
    public TextMeshProUGUI liveScoreText;
    public Image overheatEffectBackground;
    public int enemiesKilled = 0;

    private float _timer = 0;
    private int _score = 0;
    private TextMeshProUGUI _timeLivedText;
    private TextMeshProUGUI _enemiesKilledText;
    private TextMeshProUGUI _scoreText;
    private Overheating _playerOverheating;
    
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
                case "Score":
                    _scoreText = child.GetComponent<TextMeshProUGUI>();
                    break;
            }
        }

        _playerOverheating = player.GetComponent<Overheating>();
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        liveTimerText.text = _displayTimeShort(_timer);
        liveScoreText.text = CalculateScore().ToString();

        if (_playerOverheating.overheat > 70)
        {
            var color = overheatEffectBackground.color;
            var alpha = (_playerOverheating.overheat - 70) / 30 * 0.24f;
            overheatEffectBackground.color = new Color(color.r, color.g, color.b, alpha);
        }
        else
        {
            var color = overheatEffectBackground.color;
            overheatEffectBackground.color = new Color(color.r, color.g, color.b, 0);
        }
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

    private string _displayTimeShort(float timeToDisplay)
    {
        var minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        var seconds = Mathf.FloorToInt(timeToDisplay % 60);

        return $"{minutes}:{seconds:00}";
    }

    private int CalculateScore()
    {
        _score = 100 * enemiesKilled;
        _score += Math.Max(300 - Mathf.FloorToInt(_timer), 0);
        
        return _score;
    }
    
    public void YouWin()
    {
        youWinScreen.SetActive(true);

        _enemiesKilledText.text = "Enemies Killed: " + enemiesKilled;
        _timeLivedText.text = DisplayTime(_timer);
        _scoreText.text = "Score: " + CalculateScore();
        
        Time.timeScale = 0;
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }
    
}
