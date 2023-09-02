using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private int placedCount = 0;
    private int fontSize = 48;
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
        _sudokuLogic = gameObject.AddComponent<SudokuLogic>();
        Tuple<List<List<int>>, List<List<int>>> tuple = _sudokuLogic.GenerateSudoku(40);
        sudoku = tuple.Item1;
        board = tuple.Item2;
        
        buttons = GameObject.FindGameObjectWithTag("buttons");
        for (int i = 0; i < 9; i++)
        {
            List<GameObject> l = new List<GameObject>();
            List<bool> b = new();
            boxes.Add(l);
            boxFlags.Add(b);
        }
        
        for (int i = 0; i < buttons.transform.childCount; i++)
        {
            GameObject box = buttons.transform.GetChild(i).gameObject;
            for (int j = 0; j < box.transform.childCount; j++)
            {
                int row = (i - i % 3)  + j / 3;
                boxes[row].Add(box.transform.GetChild(j).gameObject);
                boxFlags[row].Add(false); // not necessary to be boxFlags[row]
            }
        }
        
        for (int i = 0; i < sudoku.Count; i++)
        {
            for (int j = 0; j < sudoku[i].Count; j++)
            {
                if (sudoku[i][j] == 0)
                {
                    boxFlags[i][j] = true;
                    continue;
                }

                TextMeshProUGUI t = boxes[i][j].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                t.text = sudoku[i][j].ToString();
                t.fontSize = fontSize;
                t.color = fixedNumberColor;

                placedCount++;
            }
        }
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
            TextMeshProUGUI t = highlightButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            if (t.text == "") placedCount++;
            
            t.text = b.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            t.fontSize = fontSize;
            t.color = numberColor;
            HighlightButton(highlightButton);
            
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
        Debug.Log("Congratulations. Game is over");
    }

}
