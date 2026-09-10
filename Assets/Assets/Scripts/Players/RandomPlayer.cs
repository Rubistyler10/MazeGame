using UnityEngine;

public class RandomPlayer : Player
{
    public override Action Think(Observation observation, int budget)
    {
        Action[] list_actions = observation.GetListActions();
        Action chosen_action = list_actions[Random.Range(0, list_actions.Length)];
        return chosen_action;
    }
}