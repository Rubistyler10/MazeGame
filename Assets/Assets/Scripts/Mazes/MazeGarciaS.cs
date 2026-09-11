using UnityEngine;

public class MazeGarciaS : Maze
{
    private void OnEnable()
    {
        Initialize(new int[,]
        {
            { 1, 0, 2, 0, 0, 0, 3, 0 },
            { 0, 0, 0, 3, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 2 },
            { 0, 0, 2, 2, 0, 3, 0, 0 },
            { 0, 3, 0, 0, 0, 0, 0, 3 },
            { 0, 3, 0, 0, 0, 2, 0, 0 },
            { 0, 0, 0, 0, 3, 3, 3, 0 },
            { 0, 0, 3, 0, 0, 0, 3, 4 }
        });
    }

    public override string ToString() => "MazeGarciaS";
}
