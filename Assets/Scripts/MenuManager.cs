using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Button continueButton;
    private void Start()
    {
        if (PlayerPrefs.HasKey("sudoku"))
        {
            continueButton.interactable = true;
        }
    }

    public void LoadNewGame(Button b)
    {
        GameState.newGame = true;
        string mode = b.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        switch (mode)
        {
            case "EASY":
                GameState.gameMode = 0;
                break;
            case "MEDIUM":
                GameState.gameMode = 1;
                break;
            case "HARD":
                GameState.gameMode = 2;
                break;
            default:
                GameState.gameMode = 0;
                break;
        }
        SceneManager.LoadScene("GameScene");
    }

    public void ContinueGame()
    {
        //Debug.Log(PlayerPrefs.GetString("sudoku"));
        //Debug.Log(PlayerPrefs.GetString("board"));
        //Debug.Log(PlayerPrefs.GetFloat("Time"));
        GameState.newGame = false;
        SceneManager.LoadScene("GameScene");
    }
    
}
