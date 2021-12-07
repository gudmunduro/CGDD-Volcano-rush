using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start : MonoBehaviour
{
    public GameObject leaderboardCanvas;
    public GameObject startCanvas;
    
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