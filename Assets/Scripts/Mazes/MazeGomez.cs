using UnityEngine;

public class MazeGomez : Maze
{
    private void OnEnable()
    {
        Initialize(new int[,]
        {
            { 1, 0, 0, 0, 2, 0, 0, 0 },
            { 0, 0, 2, 3, 3, 0, 3, 0 },
            { 0, 0, 0, 0, 3, 0, 3, 0 },
            { 0, 2, 3, 2, 2, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 2, 0 },
            { 2, 3, 2, 0, 2, 0, 2, 3 },
            { 0, 0, 0, 0, 3, 0, 0, 0 },
            { 3, 3, 0, 2, 3, 3, 0, 4 }
        });
    }

    public override string ToString() => "MazeGomez";
}
