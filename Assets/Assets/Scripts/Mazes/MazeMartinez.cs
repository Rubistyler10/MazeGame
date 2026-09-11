using UnityEngine;

public class MazeMartinez : Maze
{
    private void OnEnable()
    {
        Initialize(new int[,]
        {
            { 1, 3, 3, 3, 3, 2, 3, 3 },
            { 0, 0, 0, 2, 3, 0, 3, 0 },
            { 3, 3, 0, 0, 3, 0, 3, 0 },
            { 2, 3, 2, 0, 3, 0, 3, 0 },
            { 0, 2, 3, 0, 0, 0, 0, 0 },
            { 0, 3, 3, 0, 0, 2, 3, 0 },
            { 0, 0, 0, 3, 0, 0, 2, 0 },
            { 3, 3, 0, 0, 0, 2, 0, 0 },
            { 3, 2, 0, 3, 3, 3, 0, 0 },
            { 4, 0, 0, 0, 0, 0, 0, 2 }
        });
    }

    public override string ToString() => "MazeMartinez";
}
