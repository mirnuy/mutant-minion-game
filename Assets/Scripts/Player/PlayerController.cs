// PlayerController.cs
// Master orchestrator for the player character.
// This component lives on the root Player GameObject and holds references
// to every sub-system.  Other scripts (UI, enemies, etc.) can use
// PlayerController.Instance to query player state without tight coupling.
//
// Add this component last (after all other player components) so its Awake
// runs after the sub-systems are ready.

using UnityEngine;
using MutantMinion.Input;

namespace MutantMinion.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerCombat))]
    [RequireComponent(typeof(PlayerStamina))]
    [RequireComponent(typeof(PlayerAbility))]
    [RequireComponent(typeof(PlayerInteraction))]
    public class PlayerController : MonoBehaviour
    {
        // ─── Singleton ───────────────────────────────────────────────────────────
        public static PlayerController Instance { get; private set; }

        // ─── Sub-system references (public for other scripts to query) ────────────
        public PlayerMovement    Movement    { get; private set; }
        public PlayerCombat      Combat      { get; private set; }
        public PlayerStamina     Stamina     { get; private set; }
        public PlayerAbility     Ability     { get; private set; }
        public PlayerInteraction Interaction { get; private set; }

        // ─── Inspector ───────────────────────────────────────────────────────────
        [Header("Camera")]
        [Tooltip("Assign the PlayerCamera component (usually on the Main Camera).")]
        [SerializeField] private PlayerCamera playerCamera;

        // ─── Unity lifecycle ─────────────────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Cache sub-system references.
            Movement    = GetComponent<PlayerMovement>();
            Combat      = GetComponent<PlayerCombat>();
            Stamina     = GetComponent<PlayerStamina>();
            Ability     = GetComponent<PlayerAbility>();
            Interaction = GetComponent<PlayerInteraction>();
        }

        private void Start()
        {
            // Verify InputManager is present in the scene.
            if (InputManager.Instance == null)
                Debug.LogWarning("[PlayerController] No InputManager found in scene. " +
                                 "Please add it to the Player GameObject.");

            // Verify camera is assigned.
            if (playerCamera == null)
                Debug.LogWarning("[PlayerController] PlayerCamera not assigned. " +
                                 "Drag your Main Camera (with PlayerCamera component) into the field.");
        }

        // ─── Public API ──────────────────────────────────────────────────────────

        /// <summary>
        /// Apply a mutation to this player.
        /// Called by MinionCreation or laboratory upgrade systems.
        /// Example: controller.ApplyMutation(new WolfDNAMutation());
        /// </summary>
        public void ApplyMutation(IMutation mutation)
        {
            mutation?.Apply(this);
            Debug.Log($"[PlayerController] Mutation applied: {mutation?.GetType().Name}");
        }
    }

    // ─── Mutation interface ───────────────────────────────────────────────────────
    /// <summary>
    /// Implement this interface for each mutation type.
    /// Inject modifications through the PlayerController API.
    /// </summary>
    public interface IMutation
    {
        /// <summary>Apply mutation effects to the player controller.</summary>
        void Apply(PlayerController controller);
    }
}
