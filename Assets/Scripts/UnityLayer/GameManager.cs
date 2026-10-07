#pragma warning disable 0414
using System;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : GameSimulator
{
    [Header("Game Settings")]
    public Maze maze;
    public Player player;
    public int budget = 100;
    public int max_iterations = 100;


    [Header("Game Visualization Settings")]
    [Tooltip("Is this GameManager running without a MultiTester? If true, the GameManager will handle input and visualization. If false, the MultiTester will handle input and visualization.")]
    public bool is_standalone = false;
    [ShowIf(ActionOnConditionFail.DONT_DRAW, ConditionOperator.AND, nameof(is_standalone))]
    [Tooltip("Should the game be visualized in the Game World? If false, the game will run in the background without any visualization.")]
    public bool visualize_game = true;
    [ShowIf(ActionOnConditionFail.DONT_DRAW, ConditionOperator.AND, nameof(is_standalone))]    
    [Tooltip("Should the game auto-play without user input? If true, the game will step automatically as soon as the previous step animation is complete.")]
    public bool auto_play = false;
    [ShowIf(ActionOnConditionFail.DONT_DRAW, ConditionOperator.AND, nameof(is_standalone))]
    [Tooltip("The speed at which the game auto-plays, in steps per second.")]
    public float auto_play_speed = 1f;
    [ShowIf(ActionOnConditionFail.DONT_DRAW, ConditionOperator.AND, nameof(is_standalone))]
    [Tooltip("Should the game allow bump animations when the player tries to move into a wall? If true, the player will move slightly into the wall and then return to their original position (For flair only).")]
    public bool allow_bump_animation = false;
    [ShowIf(ActionOnConditionFail.DONT_DRAW, ConditionOperator.AND, nameof(is_standalone))]
    [Tooltip("The speed multiplier for the bump animation. A value of 1 means the bump animation will take the same amount of time as a normal step. A value of 2 means the bump animation will take half the time of a normal step.")]
    public float bump_animation_speed_multiplier = 1f;
    private bool bump_return = false;

    [Header("GameWorld Assets")]
    [SerializeField] private bool showAssetList = false;
    [ShowIf(ActionOnConditionFail.DONT_DRAW, ConditionOperator.AND, nameof(showAssetList))]
    [SerializeField] private GameObject player_prefab;
    [ShowIf(ActionOnConditionFail.DONT_DRAW, ConditionOperator.AND, nameof(showAssetList))]
    [SerializeField] private GameObject player_dead_prefab;
    [ShowIf(ActionOnConditionFail.DONT_DRAW, ConditionOperator.AND, nameof(showAssetList))]
    [SerializeField] private GameObject pursuer_player_prefab;
    [ShowIf(ActionOnConditionFail.DONT_DRAW, ConditionOperator.AND, nameof(showAssetList))]
    [SerializeField] private GameObject pursuer_player_dead_prefab;
    [ShowIf(ActionOnConditionFail.DONT_DRAW, ConditionOperator.AND, nameof(showAssetList))]
    [SerializeField] private GameObject empty_cell_prefab;
    [ShowIf(ActionOnConditionFail.DONT_DRAW, ConditionOperator.AND, nameof(showAssetList))]
    [SerializeField] private GameObject start_cell_prefab;
    [ShowIf(ActionOnConditionFail.DONT_DRAW, ConditionOperator.AND, nameof(showAssetList))]
    [SerializeField] private GameObject hole_cell_prefab;
    [ShowIf(ActionOnConditionFail.DONT_DRAW, ConditionOperator.AND, nameof(showAssetList))]
    [SerializeField] private GameObject wall_cell_prefab;
    [ShowIf(ActionOnConditionFail.DONT_DRAW, ConditionOperator.AND, nameof(showAssetList))]
    [SerializeField] private GameObject goal_cell_prefab;

    [Header("Game Information [For Debugging Purposes, Do Not Change]")]
    public bool game_ended = false;
    public int current_iteration_number = 0;

    private GameObject player_instance;
    private GameObject maze_parent;
    private Game gameScript;
    private bool is_animating = false;
    private Vector3 previous_pos = Vector3.zero;
    private Action player_action = null;
    private Vector3 target_pos = Vector3.zero;
    [HideInInspector] public bool step_pressed = false;
    private GameObject player_dead_instance;
    private InputHandler inputHandler;
    private HumanPlayer_InputHandler humanPlayer_inputHandler;
    private const float CHANGE_SPEED_AMOUNT = 1f;
    private const float STRONG_CHANGE_SPEED_AMOUNT = 2f;
    private const float MIN_AUTO_PLAY_SPEED = 1f;

    // Multiplayer mode variables
    public bool multiplayer_is_pursuer = false;
    [HideInInspector] public bool multiplayer_mode_active = false;
    [HideInInspector] public List<GameManager> pursuer_game_managers = null;

    // Start is called before the first frame update
    private void Start()
    {
        // Multiplayer children only need a Game and the player instance
        if (multiplayer_is_pursuer) {
            CreateGame();
            SpawnPlayerInstance(this.gameObject);
            return; 
        }


        if (is_standalone) SetUpInputHandler();

        // Create the underlying game
        CreateGame();
        // Spawn the game world representation of the game
        if (visualize_game) SpawnGameWorld();

        SetUpHumanPlayerInputHandler();
    }

    // Set up the input handler for the game manager so it can receive inputs, if it doesn't already exist
    private void SetUpInputHandler()
    {
        if (inputHandler == null)
        {
            inputHandler = this.gameObject.AddComponent<InputHandler>();
            inputHandler.SetInputReceiver(this);
        }
    }
    private void SetUpHumanPlayerInputHandler()
    {
        if (player is HumanPlayer)
        {
            humanPlayer_inputHandler = this.gameObject.AddComponent<HumanPlayer_InputHandler>();
            humanPlayer_inputHandler.enabled = true;
            humanPlayer_inputHandler.humanPlayer = (HumanPlayer)player;
        }
    }

    // Create the underlying game
    private void CreateGame()
    {
        gameScript = new Game();
        gameScript.SetupGame(maze, player, budget, max_iterations);
    }

    // Create the game world representation of the game, including the maze and the player
    private void SpawnGameWorld(){
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

    // Spawn the player representation in the game world at the start position of the maze
    private void SpawnPlayerInstance(GameObject parent)
    {
        // Spawn the player
        int[] start_pos = maze.GetStartPosition();
        player_instance = Instantiate(player_prefab, new Vector3(start_pos[1], 1, -start_pos[0]), Quaternion.identity);
        player_instance.transform.SetParent(parent.transform, false);
    }


    // INPUT HANDLER METHODS START
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
    /// INPUT HANDLER METHODS END


    /// Setting the changes to any settings is done as methods so the MultiTester can call them directly
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

    // Update is called once per frame
    private void Update()
    {   
        // Multiplayer children don't need to update the game, as the main player will handle that
        if (multiplayer_is_pursuer) return;
        if (!multiplayer_mode_active) 
        {
            OnUpdate();
            return;
        }


        int pursuer_animation_counter = 0;
        bool pursuer_animations_finished = pursuer_animation_counter == pursuer_game_managers.Count;

        OnUpdate(pursuer_animations_finished);
        foreach (GameManager pursuer_manager in pursuer_game_managers)
        {
            pursuer_manager.OnUpdate(pursuer_animations_finished);
            if (!pursuer_manager.is_animating) pursuer_animation_counter++;
        }
        
    }

    public void OnUpdate(bool pursuer_animations_finished = true)
    {

        if (game_ended) return;

        if (!is_animating && pursuer_animations_finished)
        {
            if (CheckForGameEnd(visualize_game)) return;

            if (auto_play || step_pressed)
            {
                // Step the game
                StepGame();
                step_pressed = false;
                if (multiplayer_mode_active && !multiplayer_is_pursuer) UpdatePursuerGoals();
            }
        }

        if (is_animating)
        {
            // Move the player instance to the new position
            MovePlayerInstance(allow_bump_animation);
            CheckAnimationEnd();
        }
    }

    private void UpdatePursuerGoals()
    {
        if (pursuer_game_managers == null) return;

        int[] main_player_pos = gameScript.GetCurrentPosition();
        foreach (GameManager pursuer_manager in pursuer_game_managers)
        {
            pursuer_manager.gameScript.UpdateGoalPosition(main_player_pos[0], main_player_pos[1]);
        }
    }

    // Check if the game has ended and handle the end of the game, including creating an instance of a dead player
    private bool CheckForGameEnd(bool visualize_game)
    {
        if (gameScript.game_ended && !game_ended)
        {
            game_ended = true;
            if (!visualize_game) return true;
            
            player_dead_instance = Instantiate(player_dead_prefab, player_instance.transform.localPosition, Quaternion.identity);

            if (is_standalone) player_dead_instance.transform.SetParent(this.transform, false);
            else player_dead_instance.transform.SetParent(this.transform.parent, false);

            player_instance.SetActive(false);
            return true;
        }
        else return false;
    }

    // Reset the game, either forcefully or only if the game has ended
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

    // Middleman for the Step() method in the Game class, to allow for animations and such
    private void StepGame()
    {
        current_iteration_number = gameScript.GetCurrentIterationNumber();

        if(!visualize_game) StepGameNoAnimation();
        else StepGameWithAnimation();
    }

    // Save information needed for the animations and step the game
    private void StepGameWithAnimation()
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

    // Step the game without any animations, for when the game is running in the background
    private void StepGameNoAnimation()
    {
        gameScript.Step();
        CheckForGameEnd(visualize_game);
    }

    // Auxiliary method to translate the position in the maze to world coordinates, assuming each cell is 1 unit in size and the maze is centered at (0, 0)
    private Vector3 TranslatePositionToWorldCoordinates(int row, int col)
    {
        // Translate the position in the maze to world coordinates
        // Assuming each cell is 1 unit in size and the maze is centered at (0, 0)
        return new Vector3(col, 1, -row);
    }

    // Move the player instance to the new position, with or without bump animation
    private void MovePlayerInstance(bool allow_bump_animation)
    {
        if (allow_bump_animation) MovePlayerInstance_BumpEnabled();
        else MovePlayerInstanceToPosition(target_pos);
    }

    /// Move the player instance to the new position, with bump animation if the player tries to move into a wall
    private void MovePlayerInstance_BumpEnabled()
    {
        if (previous_pos == target_pos && player_action != null)
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

    // Move the player instance to the new position, with a smooth transition
    private void MovePlayerInstanceToPosition(Vector3 target_pos, float animation_speed_modifier = 1f)
    {
        Debug.Log("[GAMEMANAGER][MOVEPLAYERINSTANCE] Moving player from " + previous_pos + " to " + target_pos);
        // Move the player instance to the new position, with a smooth transition
        float step = auto_play_speed * Time.deltaTime * animation_speed_modifier; // Adjust the speed as needed
        player_instance.transform.localPosition = Vector3.MoveTowards(player_instance.transform.localPosition, target_pos, step);
    }

    // Check if the player instance has reached the target position, and if so, stop animating to allow the game to continue
    private bool CheckAnimationEnd()
    {
        if (Vector3.Distance(player_instance.transform.localPosition, target_pos) < 0.01f)
        {
            is_animating = false;
            return true;
        }
        return false;
    }

    internal void AddPursuerGameManager(GameManager pursuer_manager)
    {
        if (pursuer_game_managers == null) pursuer_game_managers = new List<GameManager>();
        pursuer_game_managers.Add(pursuer_manager);
        multiplayer_mode_active = true;
    }
}
