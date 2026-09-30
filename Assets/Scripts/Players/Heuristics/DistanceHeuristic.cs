using UnityEngine;

public class DistanceHeuristic : Heuristic
{
    public override float GetScore(Observation observation)
    {
        float score;

        if (observation.IsInHole() || observation.IsInvalidPosition()) score = -1000f;
        else if (observation.ReachedGoal())
        {
            score = 1000f;
        }
        else
        {
            int row = observation.GetPosition()[0];
            int col = observation.GetPosition()[1];

            int goal_row = observation.GetGoalPosition()[0];
            int goal_col = observation.GetGoalPosition()[1];

            score = -Mathf.Sqrt(Mathf.Pow(row - goal_row, 2) + Mathf.Pow(col-goal_col, 2));
        }
        
        return score;
    }
}
