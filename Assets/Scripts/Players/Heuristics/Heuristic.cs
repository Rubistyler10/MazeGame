using UnityEngine;

public abstract class Heuristic : MonoBehaviour
{
    public abstract float GetScore(Observation observation);
}
