using System.Collections;
using UnityEngine;

public class Game 
{

    private GameState gameState;
    private ForwardModel forwardModel;
    private Queue travelled_path;
    public int final_iteration_number { get; private set; } = 0;

    private Maze maze;
    private Player player;
    private int budget;

    public bool game_ended { get; private set; } = false;
    public Game[] pursuer_games = null;
    private bool is_multiplayer_main_game ()=> pursuer_games != null;

    public void SetupGame(Maze maze, Player player, int budget, int max_iterations)
    {

        gameState = new GameState();
        forwardModel = new ForwardModel();
        travelled_path = new Queue();

        this.maze = maze;
        this.player = player;
        this.budget = budget;
        this.player.ResetPlayer();
        travelled_path.Clear();
        final_iteration_number = 0;

        gameState.ResetGameState(maze, max_iterations);
    }

    // The simulation advances when the manager decides, to allow for animations and such
    public Action Step()
    {

        Observation observation = gameState.GetObservation();
        
        Action player_action = player.Think(observation, budget);
        if (player_action == null)
        {
            if (player is HumanPlayer) return null;
            throw new System.Exception("Player returned null action");
        }
        else
        {
            forwardModel.Play(gameState, player_action);
        }

        // Save the travelled path without the start and goal, as they are assumed
        int[] currentPos = observation.GetPosition();
        if (!maze.IsGoal(currentPos[0], currentPos[1])
        && !maze.IsStart(currentPos[0], currentPos[1]))
        {
            travelled_path.Enqueue(currentPos);
        }


        if (is_multiplayer_main_game()) UpdatePursuerGoals();
        // In single player mode, we can check for terminality at the end of the step,
        // beacause the only change in state is after player action,
        // In multiplayer, however, we need to let the manager handle the terminality check,
        // as we first need every pursuer to update their game state to know if the game has ended.
        else CheckTerminality();

        return player_action;
    }

    public void CheckTerminality()
    {
        game_ended = gameState.IsTerminal(pursuer_games);
        if (game_ended)
        {   
            final_iteration_number = gameState.iteration_number;
        }
    }

    public Queue GetTravelledPath()
    {
        return travelled_path;
    }

    public int[] GetCurrentPosition()
    {
        return gameState.GetPosition();
    }

    public bool HasWon()
    {
        return gameState.HasWon();
    }

    public int GetCurrentIterationNumber()
    {
        return gameState.iteration_number;
    }

    public void UpdateGoalPosition(int row, int col)
    {
        Debug.Log($"maze exists = {maze != null} and row = {row} and col = {col} [TEST]");
        maze.SetGoalPosition(row, col);
    }

    private void UpdatePursuerGoals()
    {
        Debug.Log($"pursuer_games is null {pursuer_games == null} & pursuer_games count = {pursuer_games.Length} [TEST]");
        int[] position = gameState.GetPosition();
        foreach (Game pursuerGame in pursuer_games)
        {
            Debug.Log($"pursuerGame exists = {pursuerGame != null} and position = {position[0]},{position[1]} [TEST]");
            pursuerGame.UpdateGoalPosition(position[0], position[1]);
        }
    }

    public void SetPursuerGames(Game[] pursuerGames)
    {
        this.pursuer_games = pursuerGames;
    }
}
