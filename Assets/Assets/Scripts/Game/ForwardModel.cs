using UnityEngine;

public class ForwardModel : MonoBehaviour
{

    public void Play(GameState gameState, Action action)
    {
        int[] pos = new int[2];
        pos = gameState.GetPosition();
        int row = pos[0];
        int col = pos[1];

        switch (action)
        {
            case action.IsUp():
                row -= 1;
                break;
            case action.IsDown():
                row += 1;
                break;
            case action.IsLeft():
                col -= 1;
                break;
            case action.IsRight():
                col += 1;
                break;
            default:
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

        switch (action)
        {
            case action.IsUp():
                row -= 1;
                break;
            case action.IsDown():
                row += 1;
                break;
            case action.IsLeft():
                col -= 1;
                break;
            case action.IsRight():
                col += 1;
                break;
            default:
                throw new System.Exception("Invalid action");
        }

        observation.SetPosition(row, col);
    }

}
