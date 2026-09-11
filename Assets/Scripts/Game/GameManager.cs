using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private Maze maze;
    [SerializeField] private Player player;
    [SerializeField] private int budget = 100;
    [SerializeField] private int max_iterations = 100;

    [Header("Game Visualization Settings")]
    [SerializeField] bool visualize_game = true;
    [SerializeField] private bool auto_play = false;
    // The speed at which the game auto-plays, in steps per second
    [SerializeField] private float auto_play_speed = 1f;
    [SerializeField] private bool allow_bump_animation = false;
    [SerializeField] private float bump_animation_speed_multiplier = 1f;
    private bool bump_return = false;

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
    Vector3 previous_pos = Vector3.zero;
    Action player_action = null;
    Vector3 target_pos = Vector3.zero;
    bool game_ended = false;

    void Start()
    {
        CreateGame();
        // Spawn the game world representation of the game
        if (visualize_game) SpawnGameWorld();
    }

    void CreateGame()
    {
        // Create the underlying game
        /* GameObject game_prefab_instatiation = Instantiate(game_prefab);
        gameScript = game_prefab_instatiation.GetComponent<Game>(); */
        gameScript = new Game();
        gameScript.SetupGame(maze, player, budget, max_iterations);
    }

    void SpawnGameWorld(){
        // Create an empty parent object to hold the maze
        GameObject maze_parent = new GameObject("Maze");
        maze_parent.transform.SetParent(transform, false);
        maze_parent.transform.localPosition = Vector3.zero;

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

                GameObject cell = Instantiate(cell_prefab, new Vector3(spawn_col, 0, -spawn_row), Quaternion.identity);
                cell.transform.SetParent(maze_parent.transform, false);
            }
        }

        // Spawn the player
        int[] start_pos = maze.GetStartPosition();
        player_instance = Instantiate(player_prefab, new Vector3(start_pos[1], 1, -start_pos[0]), Quaternion.identity);
        player_instance.transform.SetParent(maze_parent.transform, false);
    }

    void Update()
    {
        if (game_ended) return;
        if (!visualize_game) gameScript.Step();

        var keyboard = Keyboard.current;

        if (!is_animating)
        {

            if (gameScript.game_ended)
            {
                game_ended = true;
                Instantiate(player_dead_prefab, player_instance.transform.position, Quaternion.identity);
                player_instance.SetActive(false);
                return;
            }


            if (auto_play || keyboard.spaceKey.wasPressedThisFrame)
            {
                // Step the game
                StepGame();
                is_animating = true;
            }
        }

        if (is_animating)
        {
            // Move the player instance to the new position
            MovePlayerInstance(allow_bump_animation);
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

    Vector3 TranslatePositionToWorldCoordinates(int row, int col)
    {
        // Translate the position in the maze to world coordinates
        // Assuming each cell is 1 unit in size and the maze is centered at (0, 0)
        return new Vector3(col, 1, -row);
    }

    void MovePlayerInstance(bool allow_bump_animation)
    {
        if (allow_bump_animation) MovePlayerInstance_BumpEnabled();
        else MovePlayerInstanceToPosition(target_pos);
    }


    void MovePlayerInstance_BumpEnabled()
    {
        if (previous_pos == target_pos)
        {
            Vector3 halfway_pos;
            if (player_action.IsUp())
            {
                halfway_pos = previous_pos + new Vector3(0, 0, 0.5f);
            }
            else if (player_action.IsDown())
            {
                halfway_pos = previous_pos + new Vector3(0, 0, -0.5f);
            }
            else if (player_action.IsLeft())
            {
                halfway_pos = previous_pos + new Vector3(-0.5f, 0, 0);
            }
            else if (player_action.IsRight())
            {
                halfway_pos = previous_pos + new Vector3(0.5f, 0, 0);
            }
            else
            {
                halfway_pos = previous_pos;
            }

            // Move the player halfway to the target position and then back to the previous position
            Debug.Log("[BUMP][GAMEMANAGER][MOVEPLAYERINSTANCE] Bumping player from " + previous_pos + " to " + target_pos);
            if (!bump_return) MovePlayerInstanceToPosition(halfway_pos, bump_animation_speed_multiplier);
            else MovePlayerInstanceToPosition(previous_pos, bump_animation_speed_multiplier);

            // Player reached bump point, begin to move to starting position of animation
            if (Vector3.Distance(player_instance.transform.localPosition, halfway_pos) < 0.01f && !bump_return)
            {
                bump_return = true;
            }
            
            // Player returning from bump, check if the animation is done
            if (CheckAnimationEnd() && bump_return)
            {
                bump_return = false;
            }
        }
        else
        {
            MovePlayerInstanceToPosition(target_pos);
            CheckAnimationEnd();
        } 
    }

    void MovePlayerInstanceToPosition(Vector3 target_pos, float animation_speed_modifier = 1f)
    {
        Debug.Log("[GAMEMANAGER][MOVEPLAYERINSTANCE] Moving player from " + previous_pos + " to " + target_pos);
        // Move the player instance to the new position, with a smooth transition
        float step = auto_play_speed * Time.deltaTime * animation_speed_modifier; // Adjust the speed as needed
        player_instance.transform.localPosition = Vector3.MoveTowards(player_instance.transform.localPosition, target_pos, step);
    }

    bool CheckAnimationEnd()
    {
        if (Vector3.Distance(player_instance.transform.localPosition, target_pos) < 0.01f)
        {
            is_animating = false;
            return true;
        }
        return false;
    }




}
