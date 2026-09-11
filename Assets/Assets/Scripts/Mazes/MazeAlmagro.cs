using UnityEngine;

public class MazeAlmagro : Maze
{
    private void OnEnable()
    {
        Initialize(new int[,]
        {
            { 0, 0, 0, 0, 2, 2, 0, 4, 2 },
            { 0, 0, 2, 0, 0, 0, 3, 0, 0 },
            { 0, 0, 0, 3, 3, 0, 3, 3, 0 },
            { 0, 2, 0, 0, 2, 0, 3, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 3, 0, 2 },
            { 2, 3, 2, 0, 2, 0, 2, 0, 2 },
            { 0, 0, 0, 0, 3, 0, 0, 0, 0 },
            { 3, 3, 0, 2, 3, 3, 0, 3, 3 },
            { 1, 0, 0, 0, 3, 3, 0, 0, 2 }
        });
    }

    public override string ToString() => "MazeAlmagro";
}
