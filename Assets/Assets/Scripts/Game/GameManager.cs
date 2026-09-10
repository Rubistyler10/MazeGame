using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game")]
    [SerializeField, InspectorName("Game Prefab")] private GameObject game_prefab; 
    [SerializeField] private Maze maze;
    [SerializeField] private Player player;

    [Header("GameWorld Assets")]
    [SerializeField] private GameObject player_prefab;
    [SerializeField] private GameObject empty_cell_prefab;
    [SerializeField] private GameObject start_cell_prefab;
    [SerializeField] private GameObject hole_cell_prefab;
    [SerializeField] private GameObject wall_cell_prefab;
    [SerializeField] private GameObject goal_cell_prefab;

    [SerializeField] bool visualize_game = true;

    void Start()
    {
        CreateGame();
        // Spawn the game world representation of the game
        if (visualize_game) SpawnGameWorld();

        
    }

    void CreateGame()
    {
        // Create the underlying game
        Game game = Instantiate(game_prefab).GetComponent<Game>();
        game.SetupGame(maze, player, 100, 100);
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
        GameObject player = Instantiate(player_prefab, new Vector3(start_pos[0], 1, start_pos[1]), Quaternion.identity);
        player.transform.parent = maze_parent.transform;


        /*
        _ _ _ _ X X _ G X 
        _ _ X _ _ _ # _ _ 
        _ _ _ # # _ # # _ 
        _ X _ _ X _ # _ _ 
        _ _ _ _ _ _ # _ X 
        X # X _ X _ X _ X 
        _ _ _ _ # _ _ _ _ 
        # # _ X # # _ # # 
        S _ _ _ # # _ _ X 
        */
    }

}
