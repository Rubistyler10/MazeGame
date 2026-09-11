using UnityEngine;

public class MazeLopez : Maze
{
    private void OnEnable()
    {
        Initialize(new int[,]
        {
            { 1, 2, 0, 0, 2, 0, 0, 0 },
            { 0, 0, 2, 0, 0, 0, 3, 0 },
            { 2, 0, 0, 0, 3, 3, 3, 0 },
            { 3, 2, 0, 0, 2, 0, 2, 0 },
            { 0, 0, 0, 0, 0, 0, 3, 0 },
            { 2, 3, 2, 0, 2, 0, 2, 0 },
            { 3, 0, 0, 0, 3, 0, 0, 2 },
            { 3, 3, 0, 2, 3, 2, 0, 4 }
        });
    }

    public override string ToString() => "MazeLopez";
}
