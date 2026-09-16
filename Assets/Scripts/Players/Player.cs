using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public abstract Action Think(Observation observation, int budget);
    public virtual void Reset()
    {}
    public override string ToString()
    {
        return "BasePlayer Script";
    }
}