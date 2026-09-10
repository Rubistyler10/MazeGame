using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField, InspectorName("Game Prefab")] private GameObject game_prefab; 
    [SerializeField] private Maze maze;
    [SerializeField] private Player player;
    [SerializeField] private int budget = 100;
    [SerializeField] private int max_iterations = 100;

    [Header("Game Visualization Settings")]
    [SerializeField] bool visualize_game = true;
    [SerializeField] private bool auto_play = false;
    // The speed at which the game auto-plays, in steps per second
    [SerializeField] private float auto_play_speed = 1f;
    private float auto_play_timer;

    [Header("GameWorld Assets")]
    [SerializeField] private GameObject player_prefab;
    [SerializeField] private GameObject player_dead_prefab;
    [SerializeField] private GameObject empty_cell_prefab;
    [SerializeField] private GameObject start_cell_prefab;
    [SerializeField] private GameObject hole_cell_prefab;
    [SerializeField] private GameObject wall_cell_prefab;
    [SerializeField] private GameObject goal_cell_prefab;


    private GameObject player_instance;
    private Game gameScript;
    private bool is_animating = false;
    private InputAction step_game_action;
    Vector3 previous_pos = Vector3.zero;
    Action player_action = null;
    Vector3 target_pos = Vector3.zero;
    bool game_ended = false;

    void OnEnable()
    {
        step_game_action = InputSystem.actions.FindAction("Game Manager/Step Game");
        step_game_action?.Enable();
    }

    void OnDisable()
    {
        step_game_action?.Disable();
    }

    void Start()
    {
        CreateGame();
        // Spawn the game world representation of the game
        if (visualize_game) SpawnGameWorld();
    }

    void CreateGame()
    {
        // Create the underlying game
        GameObject game_prefab_instatiation = Instantiate(game_prefab);
        gameScript = game_prefab_instatiation.GetComponent<Game>();
        gameScript.SetupGame(maze, player, budget, max_iterations);
    }

    void SpawnGameWorld(){
        // Create an empty parent object to hold the maze
        GameObject maze_parent = new GameObject("Maze");

        // Spawn the maze cells
        for (int spawn_row = 0; spawn_row < maze.num_rows; spawn_row++)
        {
            for (int spawn_col = maze.num_cols -1; spawn_col >= 0; spawn_col--)
            {
                GameObject cell_prefab;
                if (maze.IsStart(spawn_row, spawn_col))
                {
                    cell_prefab = start_cell_prefab;
                }
                else if (maze.IsHole(spawn_row, spawn_col))
                {
                    cell_prefab = hole_cell_prefab;
                }
                else if (maze.IsWall(spawn_row, spawn_col))
                {
                    cell_prefab = wall_cell_prefab;
                }
                else if (maze.IsGoal(spawn_row, spawn_col))
                {
                    cell_prefab = goal_cell_prefab;
                }
                else
                {
                    cell_prefab = empty_cell_prefab;
                }

                GameObject cell = Instantiate(cell_prefab, new Vector3(spawn_row, 0, spawn_col), Quaternion.identity);
                cell.transform.parent = maze_parent.transform;
            }
        }

        // Spawn the player
        int[] start_pos = maze.GetStartPosition();
        player_instance = Instantiate(player_prefab, new Vector3(start_pos[0], 1, start_pos[1]), Quaternion.identity);
        player_instance.transform.parent = maze_parent.transform;
    }

    void Update()
    {
        if (game_ended) return;
        if (!visualize_game) gameScript.Step();

        var keyboard = Keyboard.current;
        if (auto_play)
        {
            auto_play_timer += Time.deltaTime;
        }

        if (auto_play && !is_animating && auto_play_speed > 0f &&
            auto_play_timer >= 1f / auto_play_speed)
        {
            // Step the game
            StepGame();
            is_animating = true;
            auto_play_timer -= 1f / auto_play_speed;
        }
        // On Space key press, Step
        else if (keyboard.spaceKey.wasPressedThisFrame && !is_animating)
        {
            // Step the game
            StepGame();
            is_animating = true;
        }

        if (is_animating)
        {
            // Move the player instance to the new position
            MovePlayerInstance(previous_pos, target_pos);
            // Check if the player has reached the target position
            if (Vector3.Distance(player_instance.transform.position, target_pos) < 0.01f)
            {
                is_animating = false;

                if (gameScript.game_ended)
                {
                    game_ended = true;
                    Instantiate(player_dead_prefab, player_instance.transform.position, Quaternion.identity);
                    player_instance.SetActive(false);
                    return;
                }
            }
        }
        
    }

    void StepGame()
    {
        // Save the position of the player before stepping
        previous_pos = TranslatePositionToWorldCoordinates(gameScript.GetCurrentPosition()[0], gameScript.GetCurrentPosition()[1]);
        // Step the game and record the chosen action
        player_action = gameScript.Step();
        // Save the position of the player after stepping
        target_pos = TranslatePositionToWorldCoordinates(gameScript.GetCurrentPosition()[0], gameScript.GetCurrentPosition()[1]);
    }

    void MovePlayerInstance(Vector3 previous_pos, Vector3 target_pos)
    {
        // Move the player instance to the new position, with a smooth transition
        float step = 5f * Time.deltaTime; // Adjust the speed as needed
        player_instance.transform.position = Vector3.MoveTowards(player_instance.transform.position, target_pos, step);
    }

    Vector3 TranslatePositionToWorldCoordinates(int row, int col)
    {
        // Translate the position in the maze to world coordinates
        // Assuming each cell is 1 unit in size and the maze is centered at (0, 0)
        return new Vector3(row, 1, col);
    }
}
