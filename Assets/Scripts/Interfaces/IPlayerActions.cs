using static UnityEngine.InputSystem.InputAction;

public interface IPlayerActions
{
    void OnMovement(CallbackContext context);
    void OnRun(CallbackContext context);
    void OnDrop(CallbackContext context);
    void OnJump(CallbackContext context);
}
