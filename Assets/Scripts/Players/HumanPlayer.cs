using UnityEngine;

public class HumanPlayer : Player
{
    public Vector2 move_input;    
    [SerializeField] private Player helper = null;
    private bool stepHelper = false;

    public override Action Think(Observation observation, int budget)
    {


        //if (move_input == Vector2.zero) return null;

        Action chosen_action = new Action();
        if (move_input.x > 0) chosen_action.SetRight();
        else if (move_input.x < 0) chosen_action.SetLeft();
        else if (move_input.y > 0) chosen_action.SetUp();
        else if (move_input.y < 0) chosen_action.SetDown();
        else chosen_action = null;

        move_input = Vector2.zero;
        
        if (helper != null)
        {
            /* 
                Since in the Unity implementation the HumanPlayer can return null to stay still during
                realtime simulation, we can't just depend on Think() to know the advice of the helper.
                We use the seperate method Help(), that requires to know if the game should count as stepped or not,
                since it technically keeps stepping in realtime while waiting for human inputs.
            */
            stepHelper = chosen_action == null? stepHelper = false : stepHelper = true;
            Debug.Log($"[HUMANPLAYER][THINK] {helper} advises to go {helper.Help(observation, budget, stepHelper)}");
            if (stepHelper) stepHelper = false;
        }
        return chosen_action;
    }

    public override void ResetPlayer()
    {
        helper?.ResetPlayer();
    }

    public override string ToString()
    {
        return "HumanPlayer";
    }
}