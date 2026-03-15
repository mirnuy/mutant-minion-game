using UnityEngine;
using UnityEngine.InputSystem;

namespace MutantMinion.Player
{
    /// <summary>
    /// Central input manager using Unity's New Input System.
    /// Routes input events to appropriate player components.
    /// Supports remappable controls and input action asset integration.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        [Header("Input Settings")]
        [SerializeField] private bool enableInput = true;
        
        // Player components
        private PlayerMovement movement;
        private PlayerCamera playerCamera;
        private PlayerCombat combat;
        private PlayerInteraction interaction;
        private PlayerAbility ability;
        
        // Input actions (will be auto-generated from Input Action Asset)
        private PlayerInputActions inputActions;
        
        private void Awake()
        {
            // Get all player components
            movement = GetComponent<PlayerMovement>();
            playerCamera = FindObjectOfType<PlayerCamera>();
            combat = GetComponent<PlayerCombat>();
            interaction = GetComponent<PlayerInteraction>();
            ability = GetComponent<PlayerAbility>();
            
            // Initialize input actions
            inputActions = new PlayerInputActions();
        }
        
        private void OnEnable()
        {
            if (!enableInput)
                return;
            
            // Enable all input actions
            inputActions.Enable();
            
            // Subscribe to input events
            SubscribeToInputEvents();
        }
        
        private void OnDisable()
        {
            // Unsubscribe from input events
            UnsubscribeFromInputEvents();
            
            // Disable all input actions
            inputActions.Disable();
        }
        
        private void SubscribeToInputEvents()
        {
            // Movement
            if (inputActions.Player.Move != null)
                inputActions.Player.Move.performed += OnMovePerformed;
            if (inputActions.Player.Move != null)
                inputActions.Player.Move.canceled += OnMoveCanceled;
            
            // Sprint
            if (inputActions.Player.Sprint != null)
                inputActions.Player.Sprint.performed += OnSprintPerformed;
            if (inputActions.Player.Sprint != null)
                inputActions.Player.Sprint.canceled += OnSprintCanceled;
            
            // Jump
            if (inputActions.Player.Jump != null)
                inputActions.Player.Jump.performed += OnJumpPerformed;
            
            // Camera
            if (inputActions.Player.Look != null)
                inputActions.Player.Look.performed += OnLookPerformed;
            if (inputActions.Player.Look != null)
                inputActions.Player.Look.canceled += OnLookCanceled;
            
            // Combat
            if (inputActions.Player.Attack != null)
                inputActions.Player.Attack.performed += OnAttackPerformed;
            if (inputActions.Player.Block != null)
                inputActions.Player.Block.performed += OnBlockPerformed;
            if (inputActions.Player.Block != null)
                inputActions.Player.Block.canceled += OnBlockCanceled;
            
            // Interaction
            if (inputActions.Player.Interact != null)
                inputActions.Player.Interact.performed += OnInteractPerformed;
            
            // Ability
            if (inputActions.Player.Ability != null)
                inputActions.Player.Ability.performed += OnAbilityPerformed;
        }
        
        private void UnsubscribeFromInputEvents()
        {
            // Movement
            if (inputActions.Player.Move != null)
                inputActions.Player.Move.performed -= OnMovePerformed;
            if (inputActions.Player.Move != null)
                inputActions.Player.Move.canceled -= OnMoveCanceled;
            
            // Sprint
            if (inputActions.Player.Sprint != null)
                inputActions.Player.Sprint.performed -= OnSprintPerformed;
            if (inputActions.Player.Sprint != null)
                inputActions.Player.Sprint.canceled -= OnSprintCanceled;
            
            // Jump
            if (inputActions.Player.Jump != null)
                inputActions.Player.Jump.performed -= OnJumpPerformed;
            
            // Camera
            if (inputActions.Player.Look != null)
                inputActions.Player.Look.performed -= OnLookPerformed;
            if (inputActions.Player.Look != null)
                inputActions.Player.Look.canceled -= OnLookCanceled;
            
            // Combat
            if (inputActions.Player.Attack != null)
                inputActions.Player.Attack.performed -= OnAttackPerformed;
            if (inputActions.Player.Block != null)
                inputActions.Player.Block.performed -= OnBlockPerformed;
            if (inputActions.Player.Block != null)
                inputActions.Player.Block.canceled -= OnBlockCanceled;
            
            // Interaction
            if (inputActions.Player.Interact != null)
                inputActions.Player.Interact.performed -= OnInteractPerformed;
            
            // Ability
            if (inputActions.Player.Ability != null)
                inputActions.Player.Ability.performed -= OnAbilityPerformed;
        }
        
        #region Input Event Handlers
        
        // Movement
        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            if (movement != null)
                movement.OnMove(context.ReadValue<Vector2>());
        }
        
        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            if (movement != null)
                movement.OnMove(Vector2.zero);
        }
        
        // Sprint
        private void OnSprintPerformed(InputAction.CallbackContext context)
        {
            if (movement != null)
                movement.OnSprint(true);
        }
        
        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            if (movement != null)
                movement.OnSprint(false);
        }
        
        // Jump
        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            if (movement != null)
                movement.OnJump();
        }
        
        // Camera
        private void OnLookPerformed(InputAction.CallbackContext context)
        {
            if (playerCamera != null)
                playerCamera.OnLook(context.ReadValue<Vector2>());
        }
        
        private void OnLookCanceled(InputAction.CallbackContext context)
        {
            if (playerCamera != null)
                playerCamera.OnLook(Vector2.zero);
        }
        
        // Combat
        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            if (combat != null)
                combat.OnLightAttack();
        }
        
        private void OnBlockPerformed(InputAction.CallbackContext context)
        {
            if (combat != null)
                combat.OnBlock(true);
        }
        
        private void OnBlockCanceled(InputAction.CallbackContext context)
        {
            if (combat != null)
                combat.OnBlock(false);
        }
        
        // Interaction
        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            if (interaction != null)
                interaction.OnInteract();
        }
        
        // Ability
        private void OnAbilityPerformed(InputAction.CallbackContext context)
        {
            if (ability != null)
                ability.OnUseAbility();
        }
        
        #endregion
        
        /// <summary>
        /// Enable or disable input (for cutscenes, menus, etc.)
        /// </summary>
        public void SetInputEnabled(bool enabled)
        {
            enableInput = enabled;
            
            if (enabled)
            {
                inputActions.Enable();
            }
            else
            {
                inputActions.Disable();
            }
        }
        
        /// <summary>
        /// Get input actions for remapping
        /// </summary>
        public PlayerInputActions GetInputActions()
        {
            return inputActions;
        }
    }
}
