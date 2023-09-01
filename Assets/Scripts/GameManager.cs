using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    void PrintDoubleList(List<List<int>> l)
    {
        foreach (List<int> row in l)
        {
            string s = "";
            foreach (int item in row)
            {
                s += item;
            }
            Debug.Log(s);
        }
    }

    List<List<int>> CopyDoubleList(List<List<int>> l)
    {
        List<List<int>> new_l = new List<List<int>>();

        foreach (List<int> row in l)
        {
            List<int> new_row = new List<int>();
            foreach (int i in row)
            {
                new_row.Add(i);
            }
            new_l.Add(new_row);
        }

        return new_l;
    }

    /// <summary>
    /// SUDOKU SOLVER
    /// </summary>
    List<int> GetColumn(List<List<int>> l, int index)
    {
        List<int> col = new();
        for (int i = 0; i < 9; i++)
        {
            col.Add(l[i][index]);
        }
        return col;
    }
    
    List<int> Difference(List<int> l1, List<int> l2)
    {
        List<int> diff = new();

        foreach (int i in l1)
        {
            if(!l2.Contains(i)) diff.Add(i);
        }
        return diff;
    }

    List<int> GetBoxValues(List<List<int>> sudoku, int x, int y)
    {
        List<int> values = new();
        int x_base = x - (x % 3);
        int y_base = y - (y % 3);

        for (int i = x_base; i < x_base + 3; i++)
        {
            for (int j = y_base; j < y_base + 3; j++)
            {
                values.Add(sudoku[i][j]);
            }
        }

        return values;
    }
    List<int> GetPossibleValues(List<List<int>> sudoku, int x, int y)
    {
        List<int> possible = new List<int>();
        for (int i = 1; i < 10; i++)
        {
            possible.Add(i);
        }
        
        // row diff
        possible = Difference(possible, sudoku[x]);
        // column diff
        possible = Difference(possible, GetColumn(sudoku,y));
        // box diff
        possible = Difference(possible, GetBoxValues(sudoku, x, y));
        
        return possible;
    }
    void CheckSpots(List<List<int>> sudoku)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (sudoku[i][j] != 0) continue;

                List<int> possible = GetPossibleValues(sudoku, i, j);

                if (possible.Count == 0)
                {
                    Debug.Log("error. No possible value for check spots");
                }
                else if (possible.Count == 1)
                {
                    sudoku[i][j] = possible[0];
                }

            }
        }
    }

    void CheckNumbers(List<List<int>> sudoku, List<Tuple<int, int>> box_heads)
    {
        // rows
        for (int i = 0; i < 9; i++)
        {
            for (int number = 1; number < 10; number++)
            {
                if(sudoku[i].Contains(number)) continue;

                List<int> possible = new();

                for (int k = 0; k < 9; k++)
                {
                    if(sudoku[i][k] != 0 || GetColumn(sudoku, k).Contains(number) || GetBoxValues(sudoku,i,k).Contains(number)) continue;
                    
                    possible.Add(k);
                }
                
                if (possible.Count == 0)
                {
                    Debug.Log("error. No possible value for check numbers");
                }
                else if (possible.Count == 1)
                {
                    sudoku[i][possible[0]] = number;
                }            
            }
        }
        
        // columns
        for (int k = 0; k < 9; k++)
        {
            for (int number = 1; number < 10; number++)
            {
                if(GetColumn(sudoku,k).Contains(number)) continue;

                List<int> possible = new();

                for (int i = 0; i < 9; i++)
                {
                    if(sudoku[i][k] != 0 || sudoku[i].Contains(number) || GetBoxValues(sudoku,i,k).Contains(number)) continue;
                    
                    possible.Add(i);
                }
                
                if (possible.Count == 0)
                {
                    Debug.Log("error. No possible value for check numbers");
                }
                else if (possible.Count == 1)
                {
                    sudoku[possible[0]][k] = number;
                }            
            }
        }     
        // boxes
        foreach (Tuple<int,int> head in box_heads)
        {
            int x = head.Item1;
            int y = head.Item2;
            for (int number = 1; number < 10; number++)
            {
                if(GetBoxValues(sudoku,x,y).Contains(number)) continue;

                List<Tuple<int, int>> possible = new();

                for (int i = 0; i < 3; i++)
                {
                    if(sudoku[x + i].Contains(number)) continue;

                    for (int j = 0; j < 3; j++)
                    {
                        if (GetColumn(sudoku, y + j).Contains(number)) continue;
                        possible.Add(new Tuple<int, int>(x + i, y + j));
                    }
                }       
                
                if (possible.Count == 0)
                {
                    Debug.Log("error. No possible value for check numbers");
                }
                else if (possible.Count == 1)
                {
                    sudoku[possible[0].Item1][possible[0].Item2] = number;
                }  
            }
        }
    }
    bool full(List<List<int>> list)
    {
        foreach (List<int> l in list)
        {
            foreach (int i in l)
            {
                if (i == 0) return false;
            }
        }
        return true;
    }
    
    Tuple<int, List<List<int>>> SolveSudoku(List<List<int>> sudoku)
    {
        List<Tuple<int, int>> box_heads = new();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                box_heads.Add(new Tuple<int, int>(i * 3, j * 3));
            }
        }

        for (int i = 0; i < 10; i++)
        {
            CheckSpots(sudoku);
            CheckNumbers(sudoku,box_heads);
            if (full(sudoku)) return new Tuple<int, List<List<int>>>(1, sudoku);
        }
        return new Tuple<int, List<List<int>>>(0,sudoku);
    }

    /// <summary>
    /// SUDOKU GENERATOR
    /// </summary>

    Tuple<int, List<List<int>>> Generator()
    {
        
        // new_sudoku = np.zeros(shape=(9,9), dtype=int)
        List<List<int>> new_sudoku = new();
        for (int i = 0; i < 9; i++)
        {
            int[] arr = {0,0,0,0,0,0,0,0,0};
            new_sudoku.Add(arr.ToList());
        }

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                List<int> possible = GetPossibleValues(new_sudoku, i, j);
                if (possible.Count == 0)
                {
                    return new Tuple<int, List<List<int>>>(-1, new_sudoku);
                }

                int index = Random.Range(0,possible.Count);
                new_sudoku[i][j] = possible[index];
            }
        }
        return new Tuple<int, List<List<int>>>(1, new_sudoku);
    }
    
    List<List<int>> GenerateBoard()
    {
        Tuple<int, List<List<int>>> tuple = Generator();

        for (int i = 0; i < 10000; i++)
        {
            if (tuple.Item1 == 1) return tuple.Item2;
            tuple = Generator();
        }
        
        /* this while loop cause a crash in editor for some reason
        while (tuple.Item1 != 1){}
        {
            tuple = Generator();
        }
        */
        return tuple.Item2;
    }


    Tuple<int, List<List<int>>> RemoveNumber(List<List<int>> board, Tuple<int,int> location)
    {
        int x = location.Item1;
        int y = location.Item2;
        int value = board[x][y];

        board[x][y] = 0;
        
        List<List<int>> board_copy = CopyDoubleList(board);
    
        //Debug.Log("Remove number working");
        //PrintDoubleList(board);
        //Debug.Log(" ");
        Tuple<int, List<List<int>>> tuple = SolveSudoku(board_copy);
        //PrintDoubleList(board);
        //Debug.Log("Remove number done.");
        
        if (tuple.Item1 == 0) board[x][y] = value;

        return new Tuple<int, List<List<int>>>(tuple.Item1, board);

    }


    List<Tuple<int, int>> ShuffleList(List<Tuple<int, int>> l)
    {
        for (int i = 0; i < l.Count; i++)
        {
            Tuple<int, int> temp = l[i];
            int randomIndex = Random.Range(i, l.Count);
            l[i] = l[randomIndex];
            l[randomIndex] = temp;
        }
        return l;
    }
    
    List<List<int>> RemoveNumbers(List<List<int>> board, int number)
    {
        int placed = 81;
        List<Tuple<int, int>> locations = new();
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                locations.Add(new Tuple<int, int>(i,j));
            }
        }
        
        locations = ShuffleList(locations);

        while (placed > number && locations.Count > 0)
        {
            Tuple<int, List<List<int>>> tuple = RemoveNumber(board, locations[locations.Count - 1]);
            locations.RemoveAt(locations.Count - 1);
            placed -= tuple.Item1;
        }
        Debug.Log(locations.Count);

        return board;
    }

    Tuple<List<List<int>>, List<List<int>>> GenerateSudoku(int number)
    {
        List<List<int>> board = GenerateBoard();
        List<List<int>> board_copy = CopyDoubleList(board);
        List<List<int>> sudoku = RemoveNumbers(board_copy, number);
        return new Tuple<List<List<int>>, List<List<int>>>(sudoku, board);
    }


    // Start is called before the first frame update
    void Start()
    {
        Tuple<List<List<int>>, List<List<int>>> tuple = GenerateSudoku(40);
        List<List<int>> sudoku = tuple.Item1;
        List<List<int>> board = tuple.Item2;

        PrintDoubleList(sudoku);
        Debug.Log("AAAAAAAAAAAAAAAAAAAAA");
        PrintDoubleList(board);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
