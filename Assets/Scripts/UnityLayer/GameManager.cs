using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : GameSimulator
{
    [Header("Game Settings")]
    public Maze maze;
    public Player player;
    public int budget = 100;
    public int max_iterations = 100;

    [Header("Game Visualization Settings")]
    [Tooltip("Should inputs affect the game? By default this is handled by MultiTester, turn on if testing a single GameManager")]
    public bool allow_inputs = false;
    [Tooltip("Should the game be visualized in the Game World? If false, the game will run in the background without any visualization.")]
    public bool visualize_game = true;
    [Tooltip("Should the game auto-play without user input? If true, the game will step automatically as soon as the previous step animation is complete.")]
    public bool auto_play = false;
    [Tooltip("The speed at which the game auto-plays, in steps per second.")]
    public float auto_play_speed = 1f;
    [Tooltip("Should the game allow bump animations when the player tries to move into a wall? If true, the player will move slightly into the wall and then return to their original position (For flair only).")]
    public bool allow_bump_animation = false;
    [Tooltip("The speed multiplier for the bump animation. A value of 1 means the bump animation will take the same amount of time as a normal step. A value of 2 means the bump animation will take half the time of a normal step.")]
    public float bump_animation_speed_multiplier = 1f;
    private bool bump_return = false;

    [Header("GameWorld Assets")]
    [SerializeField] private GameObject player_prefab;
    [SerializeField] private GameObject player_dead_prefab;
    [SerializeField] private GameObject empty_cell_prefab;
    [SerializeField] private GameObject start_cell_prefab;
    [SerializeField] private GameObject hole_cell_prefab;
    [SerializeField] private GameObject wall_cell_prefab;
    [SerializeField] private GameObject goal_cell_prefab;

    [Header("Game Information")]
    public bool game_ended = false;
    public int current_iteration_number = 0;

    private GameObject player_instance;
    private GameObject maze_parent;
    private Game gameScript;
    private bool is_animating = false;
    Vector3 previous_pos = Vector3.zero;
    Action player_action = null;
    Vector3 target_pos = Vector3.zero;
    [HideInInspector] public bool step_pressed = false;
    private GameObject player_dead_instance;
    private HumanPlayer_InputHandler input_handler;
    const float CHANGE_SPEED_AMOUNT = 1f;
    const float STRONG_CHANGE_SPEED_AMOUNT = 2f;
    const float MIN_AUTO_PLAY_SPEED = 1f;

    void Start()
    {
        if (allow_inputs)
            SetUpInputHandler();

        CreateGame();
        // Spawn the game world representation of the game
        if (visualize_game) SpawnGameWorld();

        if (player is HumanPlayer)
        {
            // Add a new input handler script to GameManager
            input_handler = gameObject.AddComponent<HumanPlayer_InputHandler>();
            input_handler.enabled = true;
            input_handler.humanPlayer = (HumanPlayer)player;
        }
    }

    void SetUpInputHandler()
    {
        if (input_handler == null)
        {
            input_handler = this.gameObject.AddComponent<HumanPlayer_InputHandler>();
            input_handler.humanPlayer = (HumanPlayer)player;
        }
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
        maze_parent = new GameObject("Maze");
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

        SpawnPlayerInstance(maze_parent);
    }

    void SpawnPlayerInstance(GameObject maze_parent)
    {
        // Spawn the player
        int[] start_pos = maze.GetStartPosition();
        player_instance = Instantiate(player_prefab, new Vector3(start_pos[1], 1, -start_pos[0]), Quaternion.identity);
        player_instance.transform.SetParent(maze_parent.transform, false);
    }

    // Input Handling
    public override void ResetGameInputPress()
    {
        ResetGame(force_reset: false);
    }
    public override void ForceResetGameInputPress()
    {
        ResetGame(force_reset: true);
    }
    public override void ToggleAutoPlayInputPress()
    {
        ToggleAutoPlay();
    }
    public override void IncreaseAutoPlaySpeedInputPress()
    {
        ChangeAutoPlaySpeed(auto_play_speed + CHANGE_SPEED_AMOUNT);
    }
    public override void StrongIncreaseAutoPlaySpeedInputPress()
    {
        ChangeAutoPlaySpeed(auto_play_speed + STRONG_CHANGE_SPEED_AMOUNT);
    }
    public override void DecreaseAutoPlaySpeedInputPress()
    {
        ChangeAutoPlaySpeed(auto_play_speed - CHANGE_SPEED_AMOUNT);
    }
    public override void StrongDecreaseAutoPlaySpeedInputPress()
    {
        ChangeAutoPlaySpeed(auto_play_speed - STRONG_CHANGE_SPEED_AMOUNT);
    }
    public override void StepGameInputPress()
    {
        ActivateStepPressed();
    }
    public override void ToggleBumpAnimationInputPress()
    {
        ToggleBumpAnimation();
    }

    public void ToggleAutoPlay()
    {
        auto_play = !auto_play;
    }
    public void ChangeAutoPlaySpeed(float new_speed)
    {
        auto_play_speed = Mathf.Max(MIN_AUTO_PLAY_SPEED, new_speed);
    }
    public void ActivateStepPressed()
    {
        step_pressed = true;
    }
    public void ToggleBumpAnimation()
    {
        allow_bump_animation = !allow_bump_animation;
    }


    void Update()
    {   
        if (game_ended) return;
        
        if (!is_animating)
        {
            if (CheckForGameEnd(visualize_game)) return;

            if (auto_play || step_pressed)
            {
                // Step the game
                StepGame();
                step_pressed = false;
            }
        }

        if (is_animating)
        {
            // Move the player instance to the new position
            MovePlayerInstance(allow_bump_animation);
            CheckAnimationEnd();
        }
    }

    bool CheckForGameEnd(bool visualize_game)
    {
        if (gameScript.game_ended && !game_ended)
        {
            game_ended = true;
            if (!visualize_game) return true;
            
            player_dead_instance = Instantiate(player_dead_prefab, player_instance.transform.position, Quaternion.identity);
            player_dead_instance.transform.SetParent(this.transform.parent, false);
            player_instance.SetActive(false);
            return true;
        }
        else return false;
    }

    public void ResetGame(bool force_reset)
    {
        if (!force_reset && !game_ended) return;

        // Reset the game
        CreateGame();
        game_ended = false;
        if (visualize_game)
        {
            // Only need to destroy the player to respawn, maze stays the same
            Destroy(player_instance); 
            //Destroy(player_dead_instance); // MultiTester will handle destroying the player_dead_instance when it resets the game
            SpawnPlayerInstance(maze_parent);
            previous_pos = Vector3.zero;
            target_pos = Vector3.zero;
            player_action = null;
            is_animating = false;
        }
    }

    void StepGame()
    {
        current_iteration_number = gameScript.GetCurrentIterationNumber();

        if(!visualize_game) StepGameNoAnimation();
        else StepGameWithAnimation();
    }

    void StepGameWithAnimation()
    {
        // Save the position of the player before stepping
        previous_pos = TranslatePositionToWorldCoordinates(gameScript.GetCurrentPosition()[0], gameScript.GetCurrentPosition()[1]);
        // Step the game and record the chosen action
        player_action = gameScript.Step();
        // Save the position of the player after stepping
        target_pos = TranslatePositionToWorldCoordinates(gameScript.GetCurrentPosition()[0], gameScript.GetCurrentPosition()[1]);
        // Start animating
        is_animating = true;
    }

    void StepGameNoAnimation()
    {
        gameScript.Step();
        CheckForGameEnd(visualize_game);
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
