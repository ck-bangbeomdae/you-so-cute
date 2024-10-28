using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;

    private InputSystem_Actions playerActionsData;

    public static InputAction MoveAction => instance.playerActionsData.Player.Move;
    public static InputAction GravityFlipAction => instance.playerActionsData.Player.GravityFlip;
    public static InputAction InteractAction => instance.playerActionsData.Player.Interact;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;

        playerActionsData = new InputSystem_Actions();
        playerActionsData.Enable();
    }
}
