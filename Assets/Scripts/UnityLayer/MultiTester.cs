using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiTester : GameSimulator
{
    // TO DO: Implement multiple visualizations at once

    [Header("Tester Settings")]
    [Tooltip("Number of times each player should be tested on each maze")]
    [SerializeField] private int number_of_repetitions = 5;
    [SerializeField] private GameManager game_manager_prefab;
    [Tooltip("Should the tester automatically start the next game when the current one ends or wait for the input?")]
    [SerializeField] private bool auto_start_next_game_on_end = true;
    [Tooltip("Should the tester delete the previous game when starting a new one? If false, the previous game will remain in the scene disabled.")]
    [SerializeField] private bool delete_previous_game_on_next = true;
    [Tooltip("Should the tester keep the ghosts of previous games in the scene? If false, they will be deleted when starting a new game.")]
    [SerializeField] private bool keep_ghosts_of_previous_games = true;

    [Header("Game Settings")]
    [SerializeField] private Maze[] maze_list;
    [SerializeField] private Player[] player_list;
    [SerializeField] private int budget = 100;
    [SerializeField] private int max_iterations = 100;

    [Header("Game Visualization Settings")]
    [SerializeField] private bool visualize_game = true;
    // [Tooltip("Should the tester spawn several game visualizations at once in the Game World or only one at a time?")]
    // [SerializeField] private bool visualize_one_at_a_time = true; // Potential future feature, not implemented yet
    [Tooltip("Should the game auto-play without user input? If true, the game will step automatically as soon as the previous step animation is complete.")]
    [SerializeField] private bool auto_play = false;
    // The speed at which the game auto-plays, in steps per second
    [Tooltip("The speed at which the game auto-plays, in steps per second.")]
    [SerializeField] private float auto_play_speed = 1f;
    [Tooltip("Should the game allow bump animations when the player tries to move into a wall? If true, the player will move slightly into the wall and then return to their original position (For flair only).")]
    [SerializeField] private bool allow_bump_animation = false;
    [Tooltip("The speed multiplier for the bump animation. A value of 1 means the bump animation will take the same amount of time as a normal step. A value of 2 means the bump animation will take half the time of a normal step.")]
    [SerializeField] private float bump_animation_speed_multiplier = 1f;

    private GameManager current_game_simulation = null;
    private int player_index = 0;
    private int maze_index = 0;
    private int repetition_count = 0;
    private int total_game_simulations = 0;
    private int game_simulation_count = 0;
    private bool start_next_game = false;
    private InputHandler input_handler;
    private GameObject repetition_set_parent;

    const float CHANGE_SPEED_AMOUNT = 1f;
    const float STRONG_CHANGE_SPEED_AMOUNT = 2f;
    const float MIN_AUTO_PLAY_SPEED = 1f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetUpInputHandler();
        SetUpMultiTester();
    }

    void SetUpInputHandler()
    {
        if (input_handler == null)
        {
            input_handler = this.gameObject.AddComponent<InputHandler>();
            input_handler.SetInputReceiver(this);
        }
    }

    void SetUpMultiTester()
    {
        if (player_list.Length == 0)
            throw new System.Exception("No players specified for testing");
        else if (maze_list.Length == 0)
            throw new System.Exception("No mazes specified for testing");
        
        total_game_simulations = player_list.Length * maze_list.Length * number_of_repetitions;
        SimulateNextGame();
    }

    void SimulateGame()
    {
        GameManager game_manager = Instantiate(game_manager_prefab);
        game_manager.name = $"GameManager_{player_list[player_index]}_{maze_list[maze_index]}_Run{repetition_count + 1}";
        game_manager.transform.SetParent(repetition_set_parent.transform, false);
        SetUpGameManager(game_manager, maze_list[maze_index], player_list[player_index]);

        current_game_simulation = game_manager;
        repetition_count++;
        game_simulation_count++;
    }

    void SimulateNextGame()
    {
        if (game_simulation_count <= total_game_simulations)
        {
            if (repetition_count >= number_of_repetitions || game_simulation_count == 0)
            {
                if (repetition_set_parent != null && delete_previous_game_on_next)
                    Destroy(repetition_set_parent);
                
                repetition_set_parent = new GameObject($"RepetitionSet_{player_list[player_index]}_{maze_list[maze_index]}");
                repetition_set_parent.transform.SetParent(this.transform, false);

                if (game_simulation_count != 0)
                {
                    if (!keep_ghosts_of_previous_games)
                    {
                        GameObject ghost = repetition_set_parent.transform.GetChild(0).gameObject;
                        Destroy(ghost); 
                    }

                    repetition_count = 0;
                    maze_index++;
                    if (maze_index >= maze_list.Length)
                    {
                        maze_index = 0;
                        player_index++;
                        if (player_index >= player_list.Length)
                        {
                            Debug.Log("[MultiTester][TESTEND] All games have been tested");
                            current_game_simulation = null;
                            return;
                        }
                    }
                }                    
            }
            SimulateGame();
        }
    }


    void SetUpGameManager(GameManager game_manager, Maze maze, Player player)
    {
        game_manager.maze = maze;
        game_manager.player = player;
        game_manager.budget = budget;
        game_manager.max_iterations = max_iterations;
        game_manager.visualize_game = visualize_game;
        game_manager.auto_play = auto_play;
        game_manager.auto_play_speed = auto_play_speed;
        game_manager.allow_bump_animation = allow_bump_animation;
        game_manager.bump_animation_speed_multiplier = bump_animation_speed_multiplier;
        game_manager.is_standalone = false;
    }

    // Input Handling
    public override void ResetGameInputPress()
    {
        if (current_game_simulation != null)
            current_game_simulation.ResetGame(force_reset: false);
    }
    public override void ForceResetGameInputPress()
    {
        if (current_game_simulation != null)
            current_game_simulation.ResetGame(force_reset: true);
    }
    public override void ToggleAutoPlayInputPress()
    {
        auto_play = !auto_play;
        if (current_game_simulation != null)
            current_game_simulation.ToggleAutoPlay();
    }
    public override void IncreaseAutoPlaySpeedInputPress()
    {
        if (current_game_simulation != null)
            ChangeAutoPlaySpeed(CHANGE_SPEED_AMOUNT);
    }
    public override void StrongIncreaseAutoPlaySpeedInputPress()
    {
        if (current_game_simulation != null)
            ChangeAutoPlaySpeed(STRONG_CHANGE_SPEED_AMOUNT);
    }
    public override void DecreaseAutoPlaySpeedInputPress()
    {
        if (current_game_simulation != null)
            ChangeAutoPlaySpeed(-CHANGE_SPEED_AMOUNT);
    }
    public override void StrongDecreaseAutoPlaySpeedInputPress()
    {
        if (current_game_simulation != null)
            ChangeAutoPlaySpeed(-STRONG_CHANGE_SPEED_AMOUNT);
    }
    public override void StepGameInputPress()
    {
        if (current_game_simulation != null)
            current_game_simulation.step_pressed = true;
    }
    public override void ToggleBumpAnimationInputPress()
    {
        allow_bump_animation = !allow_bump_animation;
        if (current_game_simulation != null)
            current_game_simulation.allow_bump_animation = allow_bump_animation;
    }
    public override void StartNextGameInputPress()
    {
        start_next_game = true;
    }
    void ChangeAutoPlaySpeed(float delta)
    {
        auto_play_speed = Mathf.Max(MIN_AUTO_PLAY_SPEED, auto_play_speed + delta);
        if (current_game_simulation != null)
            current_game_simulation.ChangeAutoPlaySpeed(auto_play_speed);
    }


    // Update is called once per frame
    void Update()
    {
        if(current_game_simulation == null) return;

        //InputHandling();

        if (current_game_simulation.game_ended)
        {
            if (auto_start_next_game_on_end) start_next_game = true;

            if (start_next_game)
            {
                start_next_game = false;
                if (delete_previous_game_on_next) Destroy(current_game_simulation.gameObject);
                else current_game_simulation.gameObject.SetActive(false);
                SimulateNextGame();
            }
        }
    }


}
