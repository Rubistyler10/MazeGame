using UnityEngine;

public class ForwardModel : MonoBehaviour
{

    public void Play(GameState gameState, Action action)
    {
        int[] pos = new int[2];
        pos = gameState.GetPosition();
        int row = pos[0];
        int col = pos[1];

        if (action.IsUp())
        {
            row -= 1;
        }
        else if (action.IsDown())
        {
            row += 1;
        }
        else if (action.IsLeft())
        {
            col -= 1;
        }
        else if (action.IsRight())
        {
            col += 1;
        }
        else
        {
            throw new System.Exception("Invalid action");
        }

        gameState.SetPosition(row, col);
        gameState.IncrementIterationCount();
    }

    public void Test(Observation observation, Action action)
    {
        int[] pos = new int[2];
        pos = observation.GetPosition();
        int row = pos[0];
        int col = pos[1];

        if (action.IsUp())
        {
            row -= 1;
        }
        else if (action.IsDown())
        {
            row += 1;
        }
        else if (action.IsLeft())
        {
            col -= 1;
        }
        else if (action.IsRight())
        {
            col += 1;
        }
        else
        {
            throw new System.Exception("Invalid action");
        }

        observation.SetPosition(row, col);
    }

}
