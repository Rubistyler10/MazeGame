using System.Collections;
using UnityEngine;


public class Game : MonoBehaviour
{

    private GameState gameState;
    private ForwardModel forwardModel;
    private Queue travelled_path;
    private int final_iteration_number = 0;

    private Maze maze;
    private Player player;
    private int budget;
    private int max_iterations;

    public bool game_ended { get; private set; } = false;

    private Game(GameState gameState, ForwardModel forwardModel)
    {
        travelled_path = new Queue();
    }

    private void Awake()
    {
        gameState = GetComponent<GameState>();
        forwardModel = GetComponent<ForwardModel>();
    }


    private void SetupGame(Maze maze, Player player, int budget, int max_iterations)
    {
        this.maze = maze;
        this.player = player;
        this.budget = budget;
        this.max_iterations = max_iterations;

        gameState.ResetGameState(maze, max_iterations);
    }

    // The simulation advances when the manager decides, to allow for animations and such
    private void Step()
    {
        Observation observation = gameState.GetObservation();
        int[] currentPos = new int[2];
        currentPos = observation.GetPosition();
        // Save the travelled path without the start and goal, as they are assumed
        if (!maze.IsGoal(currentPos[0], currentPos[1])
        || !maze.IsStart(currentPos[0], currentPos[1]))
        {
            travelled_path.Enqueue(currentPos);
        }

        Action action = player.Think(observation, budget);
        if (action == null)
        {
            throw new System.Exception("Player returned null action");
        }
        else
        {
            forwardModel.Play(gameState, action);
        }

        game_ended = gameState.IsTerminal();
        if (game_ended)
        {
            final_iteration_number = gameState.iteration_number;
        }
    }

    private Queue GetTravelledPath()
    {
        return travelled_path;
    }

    private int GetFinalIterationNumber()
    {
        return final_iteration_number;
    }
}
