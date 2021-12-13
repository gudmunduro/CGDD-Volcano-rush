using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject youDiedScreen;
    public GameObject youWinScreen;
    public GameObject pauseGameScreen;
    public GameObject player;
    public GameObject enemies;
    public GameObject items;
    public GameObject enemyspawners;
    public GameObject itemDropPrefab;
    public GameObject powerupPrefab;
    public TextMeshProUGUI liveTimerText;
    public TextMeshProUGUI liveScoreText;
    public Image overheatEffectBackground;
    public OnlineLeaderboard onlineLeaderboard;
    public List<Vector3> defaultPowerupPositions;
    public List<Vector3> defaultItemDropPositions;
    public int enemiesKilled = 0;
    public int enemiesKilledPoints = 0;
    
    public GameObject firstSelectedYouDied;
    public GameObject firstSelectedYouWin;
    public GameObject firstSelectedPauseGame;

    public bool keyIsPressed;
    private int submitted;
    
    private float _timer = 0;
    private int _score = 0;
    private int _deaths = 0;
    private TextMeshProUGUI _timeLivedText;
    private TextMeshProUGUI _enemiesKilledText;
    private TextMeshProUGUI _scoreText;
    private Overheating _playerOverheating;
    private PlayerController2 _playercontroller;
    private AnimateObject _playerAnimate;

    private void Awake()
    {
        instance = this;

        Time.timeScale = 1f;
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
        _playercontroller = player.GetComponent<PlayerController2>();
        _playerAnimate = player.GetComponent<AnimateObject>();

        submitted = 0;
        
        CreateDefaultPickups();
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        liveTimerText.text = _displayTimeShort(_timer);
        liveScoreText.text = _calculateLiveScore().ToString();

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

        if (_playerAnimate.HealthPct() < 0.3)
        {
            if (_playerAnimate.Alive() && Time.timeScale > 0)
                SoundManager.instance.HeartVolume(0.5f - _playerAnimate.HealthPct());
            else
                SoundManager.instance.heartPlayer.Stop();
        }
    }

    public void YouDied()
    {
        youDiedScreen.SetActive(true);
        Time.timeScale = 0;
        
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedYouDied);
    }

    private String DisplayTime(float timeToDisplay)
    {
        var minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        var seconds = Mathf.FloorToInt(timeToDisplay % 60);

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
        _score = enemiesKilledPoints;
        _score += Math.Max((480 - Mathf.FloorToInt(_timer)) * 20, 0);
        
        return Math.Max(_score, 0);
    }

    private int _calculateLiveScore()
    {
        return enemiesKilledPoints;
    }
    
    public void YouWin()
    {
        SoundManager.instance.PlayComplete();
        youWinScreen.SetActive(true);

        _enemiesKilledText.text = "Enemies Killed: " + enemiesKilled;
        _timeLivedText.text = DisplayTime(_timer);
        _scoreText.text = "Score: " + CalculateScore();
        
        Time.timeScale = 0;
        
                
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedYouWin);
    }

    public void SubmitScore()
    {
        var name = GameObject.Find("LeaderboardName").GetComponent<TMP_InputField>().text;
        if (name == "") return;
        
        if (submitted == 0)
        {
            //GameObject.Find("LeaderboardName").SetActive(false);
            GameObject.Find("Submitted").GetComponent<TextMeshProUGUI>().text = "Score submitted!";
            StartCoroutine(onlineLeaderboard.SubmitEntry(name, CalculateScore(), _displayTimeShort(_timer)));
        }
        else
            GameObject.Find("Submitted").GetComponent<TextMeshProUGUI>().text = SoundManager.instance.StupidText(submitted);

        submitted++;
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Start");
    }
    
    public IEnumerator ReloadCheckpoint()
    {
        foreach (Transform child in enemies.transform) {
            Destroy(child.gameObject);
        }

        yield return new WaitForSeconds(0);

        foreach (Transform child in items.transform) {
            Destroy(child.gameObject);
        }
        CreateDefaultPickups();

        foreach(Transform child in enemyspawners.transform){
            child.GetComponent<EnemySpawner>().enemiesSpawned = false;
        }

        _deaths += 1;
        _timer += 10;
        youDiedScreen.SetActive(false);
        pauseGameScreen.SetActive(false);
        _playercontroller.Respawn();
        enemies.GetComponent<GlobalEnemyController>().enemiesAttackingPlayer = 0;
        SoundManager.instance.gameObject.SetActive(true);
        
        Time.timeScale = 1;
    }

    public void OnReloadClick()
    {
        StartCoroutine(ReloadCheckpoint());
    }

    public void UnPauseGame()
    {
        pauseGameScreen.SetActive(false);
        
        SoundManager.instance.gameObject.SetActive(true);
        Time.timeScale = 1;
        
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedPauseGame);
    }
    
    public void PauseGame()
    {
        if (pauseGameScreen.activeSelf)
        {
            UnPauseGame();
            return;
        }

        pauseGameScreen.SetActive(true);
        
        SoundManager.instance.gameObject.SetActive(false);
        Time.timeScale = 0;
        
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedPauseGame);
    }

    public void CreateDefaultPickups()
    {
        foreach (var position in defaultPowerupPositions)
        {
            Instantiate(powerupPrefab, position, Quaternion.identity, items.transform);
        }
        
        foreach (var position in defaultItemDropPositions)
        {
            var itemDrop = Instantiate(itemDropPrefab, position, Quaternion.identity, items.transform);
            itemDrop.GetComponentInChildren<ItemDrop>().healAmount = 40;
        }
    }
}
