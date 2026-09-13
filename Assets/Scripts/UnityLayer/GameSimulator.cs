using UnityEngine;

public abstract class GameSimulator : MonoBehaviour
{
    public abstract void ResetGameInputPress();
    public abstract void ForceResetGameInputPress();
    public abstract void ToggleAutoPlayInputPress();
    public abstract void IncreaseAutoPlaySpeedInputPress();
    public abstract void DecreaseAutoPlaySpeedInputPress();
    public abstract void StrongIncreaseAutoPlaySpeedInputPress();
    public abstract void StrongDecreaseAutoPlaySpeedInputPress();
    public abstract void StepGameInputPress();
    public abstract void ToggleBumpAnimationInputPress();
    // Not abstract because not all game simulators have this functionality
    public virtual void StartNextGameInputPress(){} 
}
