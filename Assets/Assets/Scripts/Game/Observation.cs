using System;
using UnityEngine;

public class Observation : MonoBehaviour
{
    int row = 0;
    int col = 0;
    Maze maze;
    public int iteration_number {get; private set;} = 0;
    int max_iterations = 0;


    public Observation(GameState gameState)
    {
        int[] pos = new int[2];
        pos = gameState.GetPosition();
        this.row = pos[0];
        this.col = pos[1];
        this.maze = gameState.maze;
        this.iteration_number = gameState.iteration_number;
        this.max_iterations = gameState.max_iterations;
    }

    public int[] GetPosition()
    {
        int[] pos = [this.row, this.col];
        return pos;
    }

    public void SetPosition(int new_row, int new_col)
    {
        if (maze.IsWall(new_row, new_col))
        {
            Debug.Log("[OBSERVATION][INVALID] Tried to move into a wall, ignoring action");
            return;
        }
        else if (new_row < 0 || new_row >= maze.rows || new_col < 0 || new_col >= maze.cols)
        {
            Debug.Log("[OBSERVATION][INVALID] Tried to move out of the maze, ignoring action");
            return;
        }
        else
        {
            this.row = new_row;
            this.col = new_col;
        }
    }

    public Action[] GetListActions()
    {
        Action[] actions = new Action[4];
        actions[0] = new Action();
        actions[0].SetUp();
        actions[1] = new Action();
        actions[1].SetRight();
        actions[2] = new Action();
        actions[2].SetDown();
        actions[3] = new Action();
        actions[3].SetLeft();
        return actions;
    }

    public bool IsTerminal()
    {
        if ((iteration_number >= max_iterations)
        ||  maze.IsGoal(row, col)
        ||  maze.IsHole(row, col))
        {
            return true;
        }
    
        else return false; 
    }

    public bool ReachedGoal()
    {
        return maze.IsGoal(row, col);
    }

    public bool IsInHole()
    {
        return maze.IsHole(row, col);
    }


}
