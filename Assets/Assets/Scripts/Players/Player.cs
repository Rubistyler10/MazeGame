using UnityEngine;

public abstract class Player : ScriptableObject
{
    public abstract Action Think(Observation observation, int budget);
    public virtual void Reset()
    {}
}