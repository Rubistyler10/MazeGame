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
    private int[] start_position = new int[2] { -1, -1 };
    private int[] goal_position = new int[2] { -1, -1 };

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
        SetStartPosition();
        SetGoalPosition();
    }


    public bool IsHole(int row, int col)
    {
        return cells[row, col] == (int)CellType.HOLE;
    }


    public bool IsWall(int row, int col)
    {
        return cells[row, col] == (int)CellType.WALL;
    }
    public bool IsStart(int row, int col)
    {
        return start_position[0] == row && start_position[1] == col;
    }
    private bool IsStartCell(int row, int col)
    {
        return cells[row, col] == (int)CellType.START;
    }

    public bool IsGoal(int row, int col)
    {
        return goal_position[0] == row && goal_position[1] == col;
    }
    private bool IsGoalCell(int row, int col)
    {
        return cells[row, col] == (int)CellType.GOAL;
    }
    public int[] GetStartPosition()
    {
        return start_position;
    }

    private void SetStartPosition()
    {
        int[] pos = new int[2];
        for (int i = 0; i < num_rows; i++)
        {
            for (int j = 0; j < num_cols; j++)
            {
                if (IsStartCell(i, j))
                {
                    pos[0] = i;
                    pos[1] = j;
                    //Debug.Log($"Start Position: ({pos[0]}, {pos[1]}) [MAZE][SETSTARTPOSITION]");
                    start_position = pos;
                    return;
                }
            }
        }
        throw new Exception("No start position found in the maze");
    }
    public void SetStartPosition(int row, int col)
    {
        if (row < 0 || row >= num_rows || col < 0 || col >= num_cols)
        {
            throw new ArgumentOutOfRangeException("Start position is out of maze bounds");
        }
        else if (IsWall(row, col))
        {
            throw new ArgumentException("Start position cannot be a wall");
        }
        else if (IsHole(row, col))
        {
            throw new ArgumentException("Start position cannot be a hole");
        }
        else
        {
            start_position[0] = row;
            start_position[1] = col;
            cells[row, col] = (int)CellType.START;
        }
    }
    public int[] GetGoalPosition()
    {
        return goal_position;
    }
    private void SetGoalPosition()
    {
        int[] pos = new int[2];
        for (int i = 0; i < num_rows; i++)
        {
            for (int j = 0; j < num_cols; j++)
            {
                if (IsGoalCell(i, j))
                {
                    pos[0] = i;
                    pos[1] = j;
                    goal_position = pos;
                    return;
                }
            }
        }
        throw new Exception("No goal position found in the maze");
    }
    public void SetGoalPosition(int row, int col)
    {
        if (row < 0 || row >= num_rows || col < 0 || col >= num_cols)
        {
            throw new ArgumentOutOfRangeException("Goal position is out of maze bounds");
        }
        else if (IsWall(row, col))
        {
            throw new ArgumentException("Goal position cannot be a wall");
        }
        /* else if (IsHole(row, col))
        {
            throw new ArgumentException("Goal position cannot be a hole");
        } */
        else
        {
            goal_position[0] = row;
            goal_position[1] = col;
            cells[row, col] = (int)CellType.GOAL;
        }
    }

    public override string ToString()
    {
        return "Base Maze Class";
    }

    internal Maze Clone()
    {
        Maze clone = ScriptableObject.CreateInstance<Maze>();
        clone.rows = this.rows;
        clone.columns = this.columns;
        clone.serializedCells = (int[])this.serializedCells.Clone();
        clone.cells = (int[,])this.cells.Clone();
        clone.start_position = (int[])this.start_position.Clone();
        clone.goal_position = (int[])this.goal_position.Clone();
        return clone;
    }
}