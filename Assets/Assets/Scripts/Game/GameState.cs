using System;
using UnityEngine;

public class GameState : MonoBehaviour
{
    private Maze maze;
    private int pos_row;
    private int pos_col;
    public int iteration_number { get; private set; } = 0;
    private int max_iterations;
    private bool terminal_is_win;

    public Observation GetObservation()
    {
        Observation observation = new Observation(this);
        return observation;
    }

    public bool IsTerminal()
    {
        if ((iteration_number >= max_iterations)
        ||  maze.IsGoal(pos_row, pos_col)
        ||  maze.IsHole(pos_row, pos_col))
        {
            return true;
        }
    
        else return false; 
    }

    public bool HasWon()
    {
        return maze.IsGoal(pos_row, pos_col);
    }

    public void Reset(Maze maze, int max_iterations)
    {
        int[] start_pos = new int[2];
        start_pos = maze.GetStartPosition();
        this.maze = maze;
        pos_row = start_pos[0];
        pos_col = start_pos[1];
        iteration_number = 0;
        this.max_iterations = max_iterations;
        terminal_is_win = false;
    }

    public void SetPosition(int new_row, int new_col)
    {
        if (maze.IsWall(new_row, new_col))
        {
            Debug.Log("[GAMESTATE][INVALID] Tried to move into a wall, ignoring action");
            return;
        }
        else if (new_row < 0 || new_row >= maze.rows || new_col < 0 || new_col >= maze.cols)
        {
            Debug.Log("[GAMESTATE][INVALID] Tried to move out of the maze, ignoring action");
            return;
        }
        else
        {
            pos_row = new_row;
            pos_col = new_col;
            
        }
    }

    public int[] GetPosition()
    {
        int[] pos = [pos_row, pos_col];
        return pos;
    }

    public void IncrementIterationCount()
    {
        iteration_number += 1;
    }

}
