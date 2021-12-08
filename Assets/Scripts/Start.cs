using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Start : MonoBehaviour
{
    public GameObject leaderboardCanvas;
    public GameObject startCanvas;

    public GameObject startFirstSelectedButton, leaderboardFirstSelectedButton, leaderboardClosedSelectedButton;
    
    public void StartGame()
    {
        SceneManager.LoadScene("Main");
        
    }

    public void OpenLeaderboard()
    {
        startCanvas.SetActive(false);
        leaderboardCanvas.SetActive(true);
    }
}