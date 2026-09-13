using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private GameSimulator input_receiver;
    private InputAction stepAction;
    private InputAction resetAction;
    private InputAction forceResetAction;
    private InputAction toggleAutoPlayAction;
    private InputAction increaseSpeedAction;
    private InputAction strongIncreaseSpeedAction;
    private InputAction decreaseSpeedAction;
    private InputAction strongDecreaseSpeedAction;
    private InputAction toggleBumpAnimationAction;
    private InputAction startNextGameAction;

    public void SetInputReceiver(GameSimulator receiver)
    {
        input_receiver = receiver;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stepAction = InputSystem.actions.FindAction("Step");
        resetAction = InputSystem.actions.FindAction("Reset");
        forceResetAction = InputSystem.actions.FindAction("Force Reset");
        toggleAutoPlayAction = InputSystem.actions.FindAction("Toggle Autoplay");
        increaseSpeedAction = InputSystem.actions.FindAction("Increase Speed");
        strongIncreaseSpeedAction = InputSystem.actions.FindAction("Strong Increase Speed");
        decreaseSpeedAction = InputSystem.actions.FindAction("Decrease Speed");
        strongDecreaseSpeedAction = InputSystem.actions.FindAction("Strong Decrease Speed");
        toggleBumpAnimationAction = InputSystem.actions.FindAction("Toggle Bump Animation");
        startNextGameAction = InputSystem.actions.FindAction("Start Next Game");
    }

    void InputHandling(){
        if (stepAction.WasPressedThisFrame())
        {
            input_receiver.StepGameInputPress();
        }
        if (resetAction.WasPressedThisFrame())
        {
            input_receiver.ResetGameInputPress();
        }
        if (forceResetAction.WasPressedThisFrame())
        {
            input_receiver.ForceResetGameInputPress();
        }
        if (toggleAutoPlayAction.WasPressedThisFrame())
        {
            input_receiver.ToggleAutoPlayInputPress();
        }
        if (increaseSpeedAction.WasPressedThisFrame())
        {
            input_receiver.IncreaseAutoPlaySpeedInputPress();
        }
        if (strongIncreaseSpeedAction.WasPressedThisFrame())
        {
            input_receiver.StrongIncreaseAutoPlaySpeedInputPress();
        }
        if (decreaseSpeedAction.WasPressedThisFrame())
        {
            input_receiver.DecreaseAutoPlaySpeedInputPress();
        }
        if (strongDecreaseSpeedAction.WasPressedThisFrame())
        {
            input_receiver.StrongDecreaseAutoPlaySpeedInputPress();
        }
        if (toggleBumpAnimationAction.WasPressedThisFrame())
        {
            input_receiver.ToggleBumpAnimationInputPress();
        }
        if (startNextGameAction.WasPressedThisFrame())
        {
            input_receiver.StartNextGameInputPress();
        }
    }

    // Update is called once per frame
    void Update()
    {
        InputHandling();
    }
}
