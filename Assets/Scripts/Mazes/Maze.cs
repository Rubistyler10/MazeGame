using System;
using UnityEngine;

public class Maze : ScriptableObject
{

    private enum CellType : int
    {
        EMPTY = 0,
        START = 1,
        HOLE = 2,
        WALL = 3,
        GOAL = 4,
        TRAVELED = 5
    };

    public int num_rows => rows;
    public int num_cols => columns;
    [SerializeField] private int rows;
    [SerializeField] private int columns;
    [SerializeField] private int[] serializedCells = Array.Empty<int>();
    private int[,] cells = new int[,]{};
    /* private int[,] cells = new int[,] {
        {0, 0, 0, 0, 2, 2, 0, 4, 2},
        {0, 0, 2, 0, 0, 0, 3, 0, 0},
        {0, 0, 0, 3, 3, 0, 3, 3, 0},
        {0, 2, 0, 0, 2, 0, 3, 0, 0},
        {0, 0, 0, 0, 0, 0, 3, 0, 2},
        {2, 3, 2, 0, 2, 0, 2, 0, 2},
        {0, 0, 0, 0, 3, 0, 0, 0, 0},
        {3, 3, 0, 2, 3, 3, 0, 3, 3},
        {1, 0, 0, 0, 3, 3, 0, 0, 2}
    }; */

    protected void Initialize(int[,] layout)
    {
        rows = layout.GetLength(0);
        columns = layout.GetLength(1);
        serializedCells = new int[rows * columns];
        cells = layout;
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                serializedCells[row * columns + column] = layout[row, column];
            }
        }
    }

    public bool IsStart(int row, int col)
    {
        return cells[row, col] == (int)CellType.START;
    }

    public bool IsHole(int row, int col)
    {
        return cells[row, col] == (int)CellType.HOLE;
    }


    public bool IsWall(int row, int col)
    {
        return cells[row, col] == (int)CellType.WALL;
    }


    public bool IsGoal(int row, int col)
    {
        return cells[row, col] == (int)CellType.GOAL;
    }

    public int[] GetStartPosition()
    {
        int[] pos = new int[2];
        for (int i = 0; i < num_rows; i++)
        {
            for (int j = 0; j < num_cols; j++)
            {
                if (IsStart(i, j))
                {
                    pos[0] = i;
                    pos[1] = j;
                    Debug.Log($"[MAZE][GETSTARTPOSITION] Start Position: ({pos[0]}, {pos[1]})");
                    return pos;
                }
            }
        }
        throw new Exception("No start position found in the maze");
    }

    public int[] GetGoalPosition()
    {
        int[] pos = new int[2];
        for (int i = 0; i < num_rows; i++)
        {
            for (int j = 0; j < num_cols; j++)
            {
                if (IsGoal(i, j))
                {
                    pos[0] = i;
                    pos[1] = j;
                    return pos;
                }
            }
        }
        throw new Exception("No goal position found in the maze");
    }

    public override string ToString()
    {
        return "Base Maze Class";
    }

}