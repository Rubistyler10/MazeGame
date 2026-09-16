public class Observation
{
    int row = 0;
    int col = 0;
    Maze maze;
    public int iteration_number {get; private set;} = 0;
    int max_iterations = 0;
    private GameState gameState;
    bool invalid_position = false;

    public Observation(GameState gameState = null)
    {
        if (gameState == null) return;
        this.gameState = gameState;
        int[] pos = gameState.GetPosition();
        row = pos[0];
        col = pos[1];
        maze = gameState.maze;
        iteration_number = gameState.iteration_number;
        max_iterations = gameState.max_iterations;
    }

    public int[] GetPosition()
    {
        int[] pos = new int[2] { row, col };
        return pos;
    }

    public void SetPosition(int new_row, int new_col)
    {  
        if (new_row < 0 || new_row >= maze.num_rows || new_col < 0 || new_col >= maze.num_cols || maze.IsWall(new_row, new_col))
        {
            invalid_position = true;
            return;
        }
        else
        {
            row = new_row;
            col = new_col;
        }
    }

    public Action[] GetListActions()
    {

        Action[] actions = new Action[4];
        actions[0] = new Action();
        actions[0].SetUp();
        actions[1] = new Action();
        actions[1].SetRight();
        actions[2] = new Action();
        actions[2].SetDown();
        actions[3] = new Action();
        actions[3].SetLeft();
        return actions;
    }

    public bool IsTerminal()
    {
        return (iteration_number >= max_iterations)
        ||  maze.IsGoal(row, col)
        ||  maze.IsHole(row, col);

    }

    public bool ReachedGoal()
    {
        return maze.IsGoal(row, col);
    }

    public bool IsInHole()
    {
        return maze.IsHole(row, col);
    }

    public bool IsInvalidPosition()
    {
        return invalid_position;
    }

    public Observation Clone()
    {
        Observation clone = new Observation(this.gameState);
        clone.gameState = this.gameState;
        clone.row = row;
        clone.col = col;
        clone.maze = maze;
        clone.iteration_number = iteration_number;
        clone.max_iterations = max_iterations;
        return clone;
    }

    public int[] GetGoalPosition()
    {
        return maze.GetGoalPosition();
    }

}
