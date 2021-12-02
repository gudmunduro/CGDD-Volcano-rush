using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEditor.SearchService.Scene;

public class Start : MonoBehaviour
{
    public SceneAsset mainScene;
    
    public void StartGame()
    {
        SceneManager.LoadScene(mainScene.name);
    }
}
