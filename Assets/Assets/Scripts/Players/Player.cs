using System.Collections;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public abstract Action Think(Observation observation, int budget);
    public void Reset()
    {}
}