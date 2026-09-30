using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public abstract Action Think(Observation observation, int budget);
    public abstract void ResetPlayer();
    public virtual Action Help(Observation observation, int budget, bool stepHelper = false)
    {
        return Think(observation, budget);
    }
    public override string ToString()
    {
        return "BasePlayer Script";
    }
}