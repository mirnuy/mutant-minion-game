// InputManager.cs
// Bridges Unity's New Input System with the rest of the game.
// All input reading is centralised here so other scripts never reference
// InputSystem directly – making it trivial to remap or add gamepad support.
//
// How to use:
//   1. Add this component to your Player GameObject (or a persistent Manager object).
//   2. Reference it from PlayerController and call the public properties each frame.

using UnityEngine;
using UnityEngine.InputSystem;

namespace MutantMinion.Input
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputManager : MonoBehaviour
    {
        // ─── Singleton ───────────────────────────────────────────────────────────
        public static InputManager Instance { get; private set; }

        // ─── Raw input values (read by other systems) ────────────────────────────
        /// <summary>WASD / left-stick movement input (x = strafe, y = forward).</summary>
        public Vector2 MoveInput   { get; private set; }

        /// <summary>Mouse delta / right-stick look input.</summary>
        public Vector2 LookInput   { get; private set; }

        /// <summary>True while Shift / left-bumper is held.</summary>
        public bool SprintHeld     { get; private set; }

        /// <summary>True on the frame Space / south-button is pressed.</summary>
        public bool JumpPressed    { get; private set; }

        /// <summary>True on the frame Left-Click / west-button is pressed (light attack).</summary>
        public bool LightAttackPressed { get; private set; }

        // Right-click / east-button is reserved for block – wired but not consumed yet.
        public bool BlockPressed   { get; private set; }

        /// <summary>True on the frame F / east-face-button is pressed (interact).</summary>
        public bool InteractPressed { get; private set; }

        /// <summary>True on the frame Q / north-face-button is pressed (ability).</summary>
        public bool AbilityPressed  { get; private set; }

        // ─── Unity lifecycle ─────────────────────────────────────────────────────
        private void Awake()
        {
            // Simple singleton – only one InputManager should exist.
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        // Consume single-frame flags at the end of each frame so they don't
        // bleed into the next Update cycle.
        private void LateUpdate()
        {
            JumpPressed         = false;
            LightAttackPressed  = false;
            BlockPressed        = false;
            InteractPressed     = false;
            AbilityPressed      = false;
        }

        // ─── Input System callbacks (auto-wired via PlayerInput component) ───────
        // The method names must match the Action names configured in
        // PlayerInputActions.inputactions (case-sensitive).

        public void OnMove(InputAction.CallbackContext ctx)
        {
            MoveInput = ctx.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext ctx)
        {
            LookInput = ctx.ReadValue<Vector2>();
        }

        public void OnSprint(InputAction.CallbackContext ctx)
        {
            SprintHeld = ctx.ReadValueAsButton();
        }

        public void OnJump(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                JumpPressed = true;
        }

        public void OnLightAttack(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                LightAttackPressed = true;
        }

        /// <summary>
        /// Right-click / block input – reserved for future block mechanic.
        /// Wired here so remapping works when block is implemented.
        /// </summary>
        public void OnBlock(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                BlockPressed = true;
        }

        public void OnInteract(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                InteractPressed = true;
        }

        public void OnAbility(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                AbilityPressed = true;
        }
    }
}
