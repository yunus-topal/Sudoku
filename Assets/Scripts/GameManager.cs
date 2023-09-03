using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI endGameText;
    public GameObject endGamePanel;
    
    private float elapsedTime = 0f;
    private int lastSaveTime = 1;
    public TMP_Text timeText;
    
    private int placedCount = 0;
    public Toggle errorToggle;
    public GameObject eraseMistakeButton;
    
    private int fontSize = 48;
    public TMP_FontAsset numberFont;
    private Color wrongNumberColor = Color.red;
    private Color numberColor = Color.blue;
    private Color fixedNumberColor = Color.black;
    
    private Button highlightButton;
    private bool canBeChanged = true;
    private Color highlightColor = new Color(0.2f, 1f, 1f, 1f);
    private Color sameNumColor = new Color(0.75f, 1f, 1f, 1f);
    
    private SudokuLogic _sudokuLogic;
    private List<List<int>> sudoku;
    private List<List<int>> board;

    private GameObject buttons;
    private List<List<GameObject>> boxes = new ();
    private List<List<bool>> boxFlags = new ();

    // Start is called before the first frame update
    void Start()
    {
        endGamePanel.SetActive(false);
        int numCount = 40 - GameState.gameMode * 5;

        if (GameState.newGame)
        {
            int count = 0;
            switch (GameState.gameMode)
            {
                case 0:
                    count = PlayerPrefs.GetInt("EasyAttempts");
                    PlayerPrefs.SetInt("EasyAttempts", count + 1);
                    break;
                case 1:
                    count = PlayerPrefs.GetInt("MediumAttempts");
                    PlayerPrefs.SetInt("MediumAttempts", count + 1);
                    break;
                case 2:
                    count = PlayerPrefs.GetInt("HardAttempts");
                    PlayerPrefs.SetInt("HardAttempts", count + 1);
                    break;
                default:
                    count = PlayerPrefs.GetInt("EasyAttempts");
                    PlayerPrefs.SetInt("EasyAttempts", count + 1);
                    break;
            }
            PlayerPrefs.Save();
            _sudokuLogic = gameObject.AddComponent<SudokuLogic>();
            Tuple<List<List<int>>, List<List<int>>> tuple = _sudokuLogic.GenerateSudoku(numCount);
            sudoku = tuple.Item1;
            board = tuple.Item2;
            
            for (int i = 0; i < 9; i++)
            {
                List<bool> b = new();
                boxFlags.Add(b);
                for (int j = 0; j < 9; j++)
                {
                    boxFlags[i].Add(sudoku[i][j] == 0);
                }
            }
        
            SaveGame();
        }
        else
        {
            LoadGame();
        }

        buttons = GameObject.FindGameObjectWithTag("buttons");
        for (int i = 0; i < 9; i++)
        {
            List<GameObject> l = new List<GameObject>();
            boxes.Add(l);
        }
        
        for (int i = 0; i < buttons.transform.childCount; i++)
        {
            GameObject box = buttons.transform.GetChild(i).gameObject;
            for (int j = 0; j < box.transform.childCount; j++)
            {
                int row = (i - i % 3)  + j / 3;
                boxes[row].Add(box.transform.GetChild(j).gameObject);
            }
        }
        
        for (int i = 0; i < sudoku.Count; i++)
        {
            for (int j = 0; j < sudoku[i].Count; j++)
            {
                if (sudoku[i][j] == 0)
                {
                    continue;
                }
                PlaceNumberInButton(boxes[i][j],sudoku[i][j].ToString(),!boxFlags[i][j]);
            }
        }
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        timeText.text = ConvertTime(elapsedTime);
        if (elapsedTime > lastSaveTime)
        {
            lastSaveTime++;
            PlayerPrefs.SetFloat("Time", elapsedTime);
            PlayerPrefs.Save();
        }
    }

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

    private string BoardToString(List<List<int>> b)
    {
        string s = "";

        foreach (List<int> list in b)
        {
            foreach (int i in list)
            {
                s += $"{i},";
            }
        }
        
        return s.Substring(0,s.Length - 1);
    }

    private List<List<int>> StringToBoard(String s)
    {
        List<List<int>> empty = new List<List<int>>();
        List<string> values = s.Split(',').ToList();
        for (int i = 0; i < 9; i++)
        {
            List<int> dummy = new();
            for (int j = 0; j < 9; j++)
            {
                dummy.Add(values[i * 9 + j][0] - 48);
            }
            empty.Add(dummy);
        }
        return empty;
    }

    private string FlagToString(List<List<bool>> b)
    {
        string s = "";

        foreach (List<bool> list in b)
        {
            foreach (bool i in list)
            {
                s += $"{(i ? 1 : 0)},";
            }
        }
        
        return s.Substring(0,s.Length - 1);
    }

    private List<List<bool>> StringToFlag(string s)
    {
        List<List<bool>> empty = new List<List<bool>>();
        List<string> values = s.Split(',').ToList();
        for (int i = 0; i < 9; i++)
        {
            List<bool> dummy = new();
            for (int j = 0; j < 9; j++)
            {
                dummy.Add(values[i * 9 + j][0] == '1');
            }
            empty.Add(dummy);
        }
        return empty;
    }

    private void SaveGame()
    {
        PlayerPrefs.SetInt("GameMode", GameState.gameMode);
        PlayerPrefs.SetFloat("Time", elapsedTime);
        PlayerPrefs.SetString("sudoku",BoardToString(sudoku));
        PlayerPrefs.SetString("board",BoardToString(board));
        PlayerPrefs.SetString("flags",FlagToString(boxFlags));
        PlayerPrefs.Save();
    }

    private void LoadGame()
    {
        elapsedTime = PlayerPrefs.GetFloat("Time");
        GameState.gameMode = PlayerPrefs.GetInt("GameMode");
        sudoku = StringToBoard(PlayerPrefs.GetString("sudoku"));
        board = StringToBoard(PlayerPrefs.GetString("board"));
        boxFlags = StringToFlag(PlayerPrefs.GetString("flags"));
    }
    
    private Tuple<int,int> GetButtonIndices(GameObject b)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (boxes[i][j].Equals(b))
                {
                    Debug.Log($"found index {i}, {j}");
                    return new Tuple<int, int>(i, j);
                }
            }
        }
        // should not be reachable
        return new Tuple<int, int>(0,0);
    }
    
    private void HighlightSameColors(string number)
    {
        foreach (List<GameObject> box in boxes)
        {
            foreach (GameObject o in box)
            {
                TextMeshProUGUI t = o.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                if (t.text != number) o.GetComponent<Image>().color  = Color.white;
                else o.GetComponent<Image>().color  = sameNumColor;
            }
        }
    }

    private void ClearBoard()
    {
        foreach (List<GameObject> box in boxes)
        {
            foreach (GameObject o in box)
            {
                o.GetComponent<Image>().color = Color.white;
            }
        }
    }
    
    public void HighlightButton(Button b)
    {
        highlightButton = b;
        Tuple<int, int> t = GetButtonIndices(b.gameObject);
        canBeChanged = boxFlags[t.Item1][t.Item2];
        
        string num = b.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        if (num.Length != 0) HighlightSameColors(num);
        else ClearBoard();
        b.GetComponent<Image>().color = highlightColor;
    }
    
    public void PutNumber(Button b)
    {
        if (canBeChanged)
        {
            string text = b.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            PlaceNumberInButton(highlightButton.gameObject,text,false);
            HighlightButton(highlightButton);
            
            Tuple<int, int> tuple = GetButtonIndices(highlightButton.gameObject);
            sudoku[tuple.Item1][tuple.Item2] = text[0] - 48;
            
            SaveGame();
            
            if(placedCount == 81) CheckGameState();

        }
    }

    private void CheckGameState()
    {
        Debug.Log("board is filled. Checking results...");
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (boxes[i][j].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text != board[i][j].ToString())
                {
                    Debug.Log("user did not filled the board correctly.");
                    return;
                }
            }
        }
        EndGame();
    }

    private void PlaceNumberInButton(GameObject box, string text ,bool fixedNumber)
    {
        TextMeshProUGUI t = box.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (t.text == "") placedCount++;
        
        t.text = text;
        t.fontSize = fontSize;
        t.font = numberFont;

        if (fixedNumber)
        {
            t.color = fixedNumberColor;
        }
        else
        {
            Tuple<int, int> tuple = GetButtonIndices(box);
            if (errorToggle.isOn && t.text != board[tuple.Item1][tuple.Item2].ToString()) t.color = wrongNumberColor;
            else t.color = numberColor;
        }
    }

    public void EraseNumber()
    {
        if (canBeChanged && highlightButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text != "")
        {
            highlightButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            HighlightButton(highlightButton);
            
            Tuple<int, int> tuple = GetButtonIndices(highlightButton.gameObject);
            sudoku[tuple.Item1][tuple.Item2] = 0;
            placedCount--;
            
            SaveGame();
        }
    }

    private void EndGame()
    {
        Debug.Log("Congratulations. Game is over");
        PlayerPrefs.DeleteKey("sudoku");
        PlayerPrefs.DeleteKey("board");
        PlayerPrefs.DeleteKey("flags");
        PlayerPrefs.Save();

        int count = 0;
        float avgTime = 0f;
        string mode = "";
        switch (GameState.gameMode)
        {
            case 0:
                mode = "easy";
                
                count = PlayerPrefs.GetInt("EasyWins");
                PlayerPrefs.SetInt("EasyWins",count + 1);
                
                avgTime = (PlayerPrefs.GetFloat("EasyWinTime") * count + elapsedTime) / (count + 1);
                PlayerPrefs.SetFloat("EasyWinTime", avgTime);
                break;
            case 1:
                mode = "medium";
                
                count = PlayerPrefs.GetInt("MediumWins");
                PlayerPrefs.SetInt("MediumWins",count + 1);
                
                avgTime = (PlayerPrefs.GetFloat("MediumWinTime") * count + elapsedTime) / (count + 1);
                PlayerPrefs.SetFloat("MediumWinTime", avgTime);
                break;
            case 2:
                mode = "hard";
                
                count = PlayerPrefs.GetInt("HardWins");
                PlayerPrefs.SetInt("HardWins",count + 1);
                
                avgTime = (PlayerPrefs.GetFloat("HardWinTime") * count + elapsedTime) / (count + 1);
                PlayerPrefs.SetFloat("HardWinTime", avgTime);
                break;
            default:
                mode = "easy";
                
                count = PlayerPrefs.GetInt("EasyWins");
                PlayerPrefs.SetInt("EasyWins",count + 1);
                
                avgTime = (PlayerPrefs.GetFloat("EasyWinTime") * count + elapsedTime) / (count + 1);
                PlayerPrefs.SetFloat("EasyWinTime", avgTime);
                break;
        }
        PlayerPrefs.Save();
        endGameText.text = $"Well Done!\nSudoku mode: {mode}\nTime: {ConvertTime(elapsedTime)}";
        endGamePanel.SetActive(true);
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void HighlightMistakes()
    {
        eraseMistakeButton.SetActive(errorToggle.isOn);
        
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                TextMeshProUGUI t = boxes[i][j].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                if (t.text != "0" && t.text != board[i][j].ToString())
                {
                    if (errorToggle.isOn) t.color = wrongNumberColor;
                    else t.color = numberColor;
                }
            }
        }
    }

    public void EraseAllMistakes()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                TextMeshProUGUI t = boxes[i][j].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                if (t.text != "" && t.color == wrongNumberColor)
                {
                    t.text = "";
                    t.color = numberColor;
                    sudoku[i][j] = 0;
                    placedCount--;
                }
            }
        }
        SaveGame();
    }
}
