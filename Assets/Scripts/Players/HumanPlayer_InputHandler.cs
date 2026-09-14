using UnityEngine;
using UnityEngine.InputSystem;

public class HumanPlayer_InputHandler : MonoBehaviour
{
    InputAction moveAction;
    Vector2 move_input;
    public HumanPlayer humanPlayer;
    private void OnEnable()
    {
        moveAction = InputSystem.actions.FindAction("MovementAxis");
    }

    // Update is called once per frame
    void Update()
    {
        if (moveAction.IsPressed())
        {
            move_input = moveAction.ReadValue<Vector2>();
            humanPlayer.move_input = move_input;
        }
        if (moveAction.WasReleasedThisFrame())
        {
            move_input = Vector2.zero;
            humanPlayer.move_input = move_input;
        }
    }
}
