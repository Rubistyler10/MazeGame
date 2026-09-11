using UnityEngine;

public class MazeTrigueros : Maze
{
    private void OnEnable()
    {
        Initialize(new int[,]
        {
            { 1, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 2, 0, 3, 0, 2, 0, 0 },
            { 2, 0, 0, 0, 3, 0, 3, 0, 0 },
            { 0, 2, 0, 3, 2, 3, 3, 0, 2 },
            { 0, 3, 0, 0, 0, 0, 3, 0, 0 },
            { 2, 3, 2, 0, 2, 0, 2, 3, 0 },
            { 0, 0, 0, 0, 3, 0, 0, 0, 0 },
            { 3, 2, 4, 2, 3, 2, 2, 0, 0 }
        });
    }

    public override string ToString() => "MazeTrigueros";
}
