using UnityEngine;

namespace Climbing
{
    public class InputCharacterController : MonoBehaviour
    {
        private PlayerControls controls = null;

        [HideInInspector] public Vector2 movement;
        [HideInInspector] public bool run;
        [HideInInspector] public bool jump;
        [HideInInspector] public bool drop;

        private void OnEnable() => controls?.Enable();

        private void OnDisable() => controls?.Disable();

        // private void ToggleRun() => run = movement.magnitude > 0.2f && !run;
        private void Exit() => Application.Quit();

        private void Awake()
        {
            //Hold and Release
            controls = new PlayerControls();
            controls.Player.Movement.performed += ctx => movement = ctx.ReadValue<Vector2>();
            controls.Player.Movement.canceled += ctx => movement = ctx.ReadValue<Vector2>();
            controls.Player.Jump.performed += ctx => jump = ctx.ReadValueAsButton();
            controls.Player.Jump.canceled += ctx => jump = ctx.ReadValueAsButton();
            controls.Player.Drop.performed += ctx => drop = ctx.ReadValueAsButton();
            controls.Player.Drop.canceled += ctx => drop = ctx.ReadValueAsButton();
            controls.Player.Run.performed += ctx => run = ctx.ReadValueAsButton();
            controls.Player.Run.canceled += ctx => run = ctx.ReadValueAsButton();
            controls.GameManager.Exit.performed += ctx => Exit();
        }
    }
}