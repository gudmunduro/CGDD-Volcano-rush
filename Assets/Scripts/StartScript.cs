using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StartScript : MonoBehaviour
{
    public GameObject leaderboardCanvas;
    public GameObject startCanvas;
    public GameObject startScreen;
    public GameObject controlsCanvas;
    
    public GameObject startFirstSelectedButton, leaderboardFirstSelectedButton, leaderboardClosedSelectedButton, controlsFirstSelectedButton, controlsClosedSelectedButton;
    
    public void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(startFirstSelectedButton);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Main");
        
    }

    public void OpenLeaderboard()
    {
        startCanvas.SetActive(false);
        leaderboardCanvas.SetActive(true);
        
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(leaderboardFirstSelectedButton);
        
    }

    public void CloseLeaderBoard()
    {
        leaderboardCanvas.SetActive(false);
        startCanvas.SetActive(true);
        
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(leaderboardClosedSelectedButton);
    }

    public void OpenControls()
    {
        controlsCanvas.SetActive(true);
        startScreen.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(controlsFirstSelectedButton);
    }

    public void CloseControls()
    {
        startScreen.SetActive(true);
        controlsCanvas.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(controlsClosedSelectedButton);
    }
}