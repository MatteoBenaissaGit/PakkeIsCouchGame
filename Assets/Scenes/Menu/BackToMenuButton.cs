using System;
using System.Collections;
using System.Collections.Generic;
using Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenuButton : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoBackToMenu();
        }
    }

    public void GoBackToMenu()
    {
        Debug.Log("menu");
        if (MultiplayerManager.Instance != null)
        {
            Destroy(MultiplayerManager.Instance.gameObject);
            MultiplayerManager.Instance = null;
        }
        SceneManager.LoadScene("MenuScene");
    }
}
