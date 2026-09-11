using UnityEngine;

public class MazeIzquierdo : Maze
{
    private void OnEnable()
    {
        Initialize(new int[,]
        {
            { 1, 0, 0, 0, 2, 0, 0, 0 },
            { 0, 3, 0, 0, 0, 0, 3, 0 },
            { 0, 3, 0, 0, 0, 0, 3, 0 },
            { 0, 0, 0, 0, 0, 2, 0, 0 },
            { 0, 0, 2, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 2, 0, 0, 4 }
        });
    }

    public override string ToString() => "MazeIzquierdo";
}
