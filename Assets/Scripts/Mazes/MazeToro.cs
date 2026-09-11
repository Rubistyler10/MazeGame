using UnityEngine;

public class MazeToro : Maze
{
    private void OnEnable()
    {
        Initialize(new int[,]
        {
            { 0, 0, 0, 3, 0, 0, 0, 3, 1 },
            { 0, 3, 0, 3, 0, 3, 0, 2, 0 },
            { 0, 3, 0, 3, 0, 3, 0, 3, 0 },
            { 0, 3, 0, 2, 0, 2, 0, 2, 0 },
            { 0, 2, 0, 3, 0, 3, 0, 2, 0 },
            { 0, 3, 0, 3, 0, 3, 0, 3, 0 },
            { 0, 3, 0, 3, 0, 2, 0, 2, 0 },
            { 0, 3, 0, 2, 0, 3, 0, 3, 0 },
            { 4, 3, 0, 0, 0, 3, 0, 0, 0 }
        });
    }

    public override string ToString() => "MazeToro";
}
