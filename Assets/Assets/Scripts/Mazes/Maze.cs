using System;
using UnityEngine;

public class Maze : MonoBehaviour
{

    private enum CellType : int
    {
        EMPTY = 0,
        START = 1,
        HOLE = 2,
        WALL = 3,
        GOAL = 4,
        TRAVELED = 5
    }

    public int num_rows { get; private set; }
    public int num_cols { get; private set; }
    public int[,] cells { get; private set; }

    public bool IsStart(int row, int col)
    {
        if (cells[row, col] == (int)CellType.START)
            return true;

        else return false;
    }

    public bool IsHole(int row, int col)
    {
        if (cells[row, col] == (int)CellType.HOLE)
            return true;

        else return false;
    }


    public bool IsWall(int row, int col)
    {
        if (cells[row, col] == (int)CellType.WALL)
            return true;

        else return false;
    }


    public bool IsGoal(int row, int col)
    {
        if (cells[row, col] == (int)CellType.GOAL)
            return true;

        else return false;
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
                    return pos;
                }
            }
        }
        throw new System.Exception("No start position found in the maze");
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
        throw new System.Exception("No goal position found in the maze");
    }

    public override string ToString()
    {
        return "Base Maze Class";
    }

}