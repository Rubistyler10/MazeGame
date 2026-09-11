using System.Collections;
using UnityEngine;


public class Game : MonoBehaviour
{

    private GameState gameState;
    private ForwardModel forwardModel;
    private Queue travelled_path;
    public int final_iteration_number { get; private set; } = 0;

    private Maze maze;
    private Player player;
    private int budget;

    public bool game_ended { get; private set; } = false;

    private void Awake()
    {
        gameState = new GameState();
        forwardModel = new ForwardModel();
        travelled_path = new Queue();
    }


    public void SetupGame(Maze maze, Player player, int budget, int max_iterations)
    {
        this.maze = maze;
        this.player = player;
        this.budget = budget;
        this.player.Reset();
        travelled_path.Clear();
        final_iteration_number = 0;

        gameState.ResetGameState(maze, max_iterations);
    }

    // The simulation advances when the manager decides, to allow for animations and such
    public Action Step()
    {
        Observation observation = gameState.GetObservation();
        int[] currentPos = observation.GetPosition();
        // Save the travelled path without the start and goal, as they are assumed
        if (!maze.IsGoal(currentPos[0], currentPos[1])
        && !maze.IsStart(currentPos[0], currentPos[1]))
        {
            travelled_path.Enqueue(currentPos);
        }

        Action player_action = player.Think(observation, budget);
        if (player_action == null)
        {
            throw new System.Exception("Player returned null action");
        }
        else
        {
            forwardModel.Play(gameState, player_action);
        }

        game_ended = gameState.IsTerminal();
        if (game_ended)
        {
            final_iteration_number = gameState.iteration_number;
        }

        return player_action;
    }

    public Queue GetTravelledPath()
    {
        return travelled_path;
    }

    public int[] GetCurrentPosition()
    {
        return gameState.GetPosition();
    }
}
