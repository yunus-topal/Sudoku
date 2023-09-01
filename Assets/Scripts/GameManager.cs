using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

    private SudokuLogic _sudokuLogic;
    private List<List<int>> sudoku;
    private List<List<int>> board;

    private GameObject buttons;
    private List<List<GameObject>> boxes = new ();


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
            boxes.Add(l);
        }
        
        for (int i = 0; i < buttons.transform.childCount; i++)
        {
            GameObject box = buttons.transform.GetChild(i).gameObject;
            for (int j = 0; j < box.transform.childCount; j++)
            {
                int row = (i - i % 3)  + j / 3;
                boxes[row].Add(box.transform.GetChild(j).gameObject);
                //square.Add(box.transform.GetChild(j).gameObject);
            }
        }
        
        for (int i = 0; i < sudoku.Count; i++)
        {
            for (int j = 0; j < sudoku[i].Count; j++)
            {
                TextMeshProUGUI t = boxes[i][j].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                t.text = sudoku[i][j].ToString();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
