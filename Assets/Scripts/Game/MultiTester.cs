using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiTester : MonoBehaviour
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
    [Tooltip("Should the tester spawn several game visualizations at once in the Game World or only one at a time?")]
    // [SerializeField] private bool visualize_one_at_a_time = true; // Potential future feature, not implemented yet
    [SerializeField] private bool auto_play = false;
    // The speed at which the game auto-plays, in steps per second
    [SerializeField] private float auto_play_speed = 1f;
    [SerializeField] private bool allow_bump_animation = false;
    [SerializeField] private float bump_animation_speed_multiplier = 1f;

    private GameManager current_game_simulation = null;
    private int player_index = 0;
    private int maze_index = 0;
    private int repetition_count = 0;
    private int total_game_simulations = 0;
    private int game_simulation_count = 0;
    private bool start_next_game = false;

    private InputAction stepAction;
    private InputAction resetAction;
    private InputAction forceResetAction;
    private InputAction toggleAutoPlayAction;
    private InputAction increaseSpeedAction;
    private InputAction decreaseSpeedAction;
    private InputAction toggleBumpAnimationAction;
    private InputAction startNextGameAction;
    private GameObject repetition_set_parent;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stepAction = InputSystem.actions.FindAction("Step");
        resetAction = InputSystem.actions.FindAction("Reset");
        forceResetAction = InputSystem.actions.FindAction("Force Reset");
        toggleAutoPlayAction = InputSystem.actions.FindAction("Toggle Autoplay");
        increaseSpeedAction = InputSystem.actions.FindAction("Increase Speed");
        decreaseSpeedAction = InputSystem.actions.FindAction("Decrease Speed");
        toggleBumpAnimationAction = InputSystem.actions.FindAction("Toggle Bump Animation");
        startNextGameAction = InputSystem.actions.FindAction("Start Next Game");
        SetUpMultiTester();
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
        game_manager.allow_inputs = false;
    }


    void InputHandling()
    {
        if (forceResetAction.WasPressedThisFrame())
            current_game_simulation.ResetGame();
        if(resetAction.WasPressedThisFrame() && current_game_simulation.game_ended)
            current_game_simulation.ResetGame();
        if (toggleAutoPlayAction.WasPressedThisFrame())
        {
            auto_play = !auto_play;
            current_game_simulation.auto_play = auto_play;
        }
        if (increaseSpeedAction.WasPressedThisFrame())
        {
            auto_play_speed += 0.5f;
            current_game_simulation.auto_play_speed = auto_play_speed;
        }
        if (decreaseSpeedAction.WasPressedThisFrame())
        {
            auto_play_speed = Mathf.Max(0.5f, auto_play_speed - 0.5f);
            current_game_simulation.auto_play_speed = auto_play_speed;
        }
        if (stepAction.WasPressedThisFrame())
            current_game_simulation.step_pressed = true;
        if (toggleBumpAnimationAction.WasPressedThisFrame()){
            allow_bump_animation = !allow_bump_animation;
            current_game_simulation.allow_bump_animation = allow_bump_animation;
        }
        if (startNextGameAction.WasPressedThisFrame())
            start_next_game = true;

    }

    // Update is called once per frame
    void Update()
    {
        if(current_game_simulation == null) return;

        InputHandling();

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
