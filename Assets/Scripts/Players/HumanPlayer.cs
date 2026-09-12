using UnityEngine;

public class HumanPlayer : Player
{
    /*  
        Input polling should be owned by GameManager, a separate MonoBehaviour, or replaced with an input callback.
        The HumanPlayer should then consume the stored input from Think(). 
    */
    public Vector2 move_input;    

    public override Action Think(Observation observation, int budget)
    {
        if (move_input == Vector2.zero) return null;

        Action chosen_action = new Action();
        if (move_input.x > 0) chosen_action.SetRight();
        else if (move_input.x < 0) chosen_action.SetLeft();
        else if (move_input.y > 0) chosen_action.SetUp();
        else if (move_input.y < 0) chosen_action.SetDown();

        move_input = Vector2.zero;
        return chosen_action;
    }

    public override string ToString()
    {
        return "HumanPlayer";
    }
}