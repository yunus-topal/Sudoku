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
    public GameObject statsPanel;
    public Button continueButton;
    public TextMeshProUGUI easyStats;
    public TextMeshProUGUI mediumStats;
    public TextMeshProUGUI hardStats;

    public GameObject difficultyOptions;
    
    private void Start()
    {
        difficultyOptions.SetActive(false);
        statsPanel.SetActive(false);
        if (PlayerPrefs.HasKey("sudoku"))
        {
            continueButton.interactable = true;
        }

        if (!PlayerPrefs.HasKey("EasyWins"))
        {
            ResetStats();
        }
        
        SetStats();
        
    }

    // Code duplication move to util class
    private string ConvertTime(float f)
    {
        int x = (int) f;
        int seconds = x % 60;
        int total_minutes = x / 60;
        int minutes = total_minutes % 60;
        int hours = total_minutes / 60;

        string sec = (seconds >= 10) ? seconds.ToString() : "0" + seconds;
        string min = minutes >= 10 ? minutes.ToString() : "0" + minutes;
        if (hours == 0) return $"{min}:{sec}";
        
        return $"{hours}:{min}:{sec}";
    }
    
    private void SetStats()
    {
        easyStats.text = String.Format("Easy\nAttempts: {0}\nCompletions: {1}\nAverage Completion Time: {2}",PlayerPrefs.GetInt("EasyAttempts"),PlayerPrefs.GetInt("EasyWins"),ConvertTime(PlayerPrefs.GetFloat("EasyWinTime")));
        mediumStats.text = String.Format("Medium\nAttempts: {0}\nCompletions: {1}\nAverage Completion Time: {2}",PlayerPrefs.GetInt("MediumAttempts"),PlayerPrefs.GetInt("MediumWins"),ConvertTime(PlayerPrefs.GetFloat("MediumWinTime")));
        hardStats.text = String.Format("Hard\nAttempts: {0}\nCompletions: {1}\nAverage Completion Time: {2}",PlayerPrefs.GetInt("HardAttempts"),PlayerPrefs.GetInt("HardWins"),ConvertTime(PlayerPrefs.GetFloat("HardWinTime")));
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

    public void ResetStats()
    {
        PlayerPrefs.SetInt("EasyWins",0);
        PlayerPrefs.SetFloat("EasyWinTime",0f);
        PlayerPrefs.SetInt("EasyAttempts",0);
            
        PlayerPrefs.SetInt("MediumWins",0);
        PlayerPrefs.SetFloat("MediumWinTime",0f);
        PlayerPrefs.SetInt("MediumAttempts",0);
            
        PlayerPrefs.SetInt("HardWins",0);
        PlayerPrefs.SetFloat("HardWinTime",0f);
        PlayerPrefs.SetInt("HardAttempts",0);
        
        SetStats();
    }
}
