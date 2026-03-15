using UnityEngine;
using MutantMinion.Player;

/// <summary>
/// Main player controller that orchestrates all player systems.
/// This is the central hub for the modular character controller.
/// Attach this to your player GameObject along with all required components.
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerStamina))]
[RequireComponent(typeof(PlayerCombat))]
[RequireComponent(typeof(PlayerInteraction))]
[RequireComponent(typeof(PlayerAbility))]
[RequireComponent(typeof(InputManager))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Components")]
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerStamina stamina;
    [SerializeField] private PlayerCombat combat;
    [SerializeField] private PlayerInteraction interaction;
    [SerializeField] private PlayerAbility ability;
    [SerializeField] private InputManager inputManager;

    [Header("Debug Info")]
    [SerializeField] private bool showDebugInfo = true;

    // Properties for external access
    public PlayerMovement Movement => movement;
    public PlayerStamina Stamina => stamina;
    public PlayerCombat Combat => combat;
    public PlayerInteraction Interaction => interaction;
    public PlayerAbility Ability => ability;

    private void Awake()
    {
        // Auto-assign components if not already assigned in Inspector
        if (movement == null) movement = GetComponent<PlayerMovement>();
        if (stamina == null) stamina = GetComponent<PlayerStamina>();
        if (combat == null) combat = GetComponent<PlayerCombat>();
        if (interaction == null) interaction = GetComponent<PlayerInteraction>();
        if (ability == null) ability = GetComponent<PlayerAbility>();
        if (inputManager == null) inputManager = GetComponent<InputManager>();

        // Verify all components are present
        if (movement == null) Debug.LogError("PlayerController: PlayerMovement component missing!");
        if (stamina == null) Debug.LogError("PlayerController: PlayerStamina component missing!");
        if (combat == null) Debug.LogError("PlayerController: PlayerCombat component missing!");
        if (interaction == null) Debug.LogError("PlayerController: PlayerInteraction component missing!");
        if (ability == null) Debug.LogError("PlayerController: PlayerAbility component missing!");
        if (inputManager == null) Debug.LogError("PlayerController: InputManager component missing!");
    }

    private void Start()
    {
        // Subscribe to events for debugging/UI
        if (stamina != null)
        {
            stamina.OnStaminaChanged += OnStaminaChanged;
        }

        if (ability != null)
        {
            ability.OnAbilityActivated += OnAbilityActivated;
            ability.OnAbilityDeactivated += OnAbilityDeactivated;
        }

        if (showDebugInfo)
        {
            Debug.Log("Player Controller initialized successfully!");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (stamina != null)
        {
            stamina.OnStaminaChanged -= OnStaminaChanged;
        }

        if (ability != null)
        {
            ability.OnAbilityActivated -= OnAbilityActivated;
            ability.OnAbilityDeactivated -= OnAbilityDeactivated;
        }
    }

    // Event handlers (can be used for UI updates)
    private void OnStaminaChanged(float current, float max)
    {
        if (showDebugInfo)
        {
            Debug.Log($"Stamina: {current:F0}/{max:F0}");
        }
    }

    private void OnAbilityActivated(AbilityData abilityData)
    {
        if (showDebugInfo)
        {
            Debug.Log($"Ability Activated: {abilityData.abilityName}");
        }
    }

    private void OnAbilityDeactivated(AbilityData abilityData)
    {
        if (showDebugInfo)
        {
            Debug.Log($"Ability Deactivated: {abilityData.abilityName}");
        }
    }

    /// <summary>
    /// Get player health (placeholder for future health system)
    /// </summary>
    public float GetHealth()
    {
        // TODO: Implement health system
        return 100f;
    }

    /// <summary>
    /// Enable/disable player controls (for cutscenes, menus, etc.)
    /// </summary>
    public void SetControlsEnabled(bool enabled)
    {
        if (inputManager != null)
        {
            inputManager.SetInputEnabled(enabled);
        }
    }
}
