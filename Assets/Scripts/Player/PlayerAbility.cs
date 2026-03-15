// PlayerAbility.cs
// Single ability slot that mutations can populate at runtime.
// The slot can hold one IAbility at a time; pressing Q activates it.
//
// How mutations use this:
//   • At minion-creation time, call SetAbility(new WolfSpeedAbility()) etc.
//   • The ability handles its own logic; this component only manages
//     activation, cooldown, and stamina gating.
//
// Built-in placeholder ability (debug print) is used until a real ability
// is injected, so the button is always functional for testing.

using System;
using UnityEngine;
using MutantMinion.Input;
using MutantMinion.Utils;

namespace MutantMinion.Player
{
    // ─── Ability interface ────────────────────────────────────────────────────────
    /// <summary>
    /// Implement this interface to create a new mutation ability.
    /// Inject via PlayerAbility.SetAbility().
    /// </summary>
    public interface IAbility
    {
        /// <summary>Human-readable name shown in HUD.</summary>
        string Name { get; }

        /// <summary>Cooldown in seconds.</summary>
        float Cooldown { get; }

        /// <summary>Stamina cost per activation.</summary>
        float StaminaCost { get; }

        /// <summary>Called when the player activates the ability.</summary>
        void Activate(GameObject owner);
    }

    // ─── Default placeholder ability ─────────────────────────────────────────────
    /// <summary>
    /// Stand-in ability used when no mutation is equipped.
    /// Logs a message so you can confirm input works without a real ability.
    /// </summary>
    public class PlaceholderAbility : IAbility
    {
        public string Name        => "Ability Slot (Empty)";
        public float  Cooldown    => Constants.DEFAULT_ABILITY_COOLDOWN;
        public float  StaminaCost => Constants.ABILITY_STAMINA_COST;

        public void Activate(GameObject owner)
        {
            Debug.Log("[Ability] No ability equipped. Assign a mutation to use this slot.");
        }
    }

    // ─── PlayerAbility component ──────────────────────────────────────────────────
    public class PlayerAbility : MonoBehaviour
    {
        // ─── Inspector ───────────────────────────────────────────────────────────
        [Header("Ability Slot")]
        [Tooltip("Override default cooldown (0 = use the ability's own cooldown).")]
        [SerializeField] private float cooldownOverride = 0f;

        // ─── References ──────────────────────────────────────────────────────────
        private PlayerStamina _stamina;

        // ─── State ───────────────────────────────────────────────────────────────
        private IAbility _currentAbility;
        private float    _cooldownTimer;

        // ─── Events ──────────────────────────────────────────────────────────────
        public event Action<string, float> OnAbilityActivated; // name, cooldown

        // ─── Public properties ───────────────────────────────────────────────────
        public bool  IsOnCooldown  => _cooldownTimer > 0f;
        public float CooldownLeft  => Mathf.Max(0f, _cooldownTimer);
        public float CooldownTotal => _currentAbility?.Cooldown ?? 0f;
        public string AbilityName  => _currentAbility?.Name ?? "None";

        // ─── Unity lifecycle ─────────────────────────────────────────────────────
        private void Awake()
        {
            _stamina        = GetComponent<PlayerStamina>();
            _currentAbility = new PlaceholderAbility();
        }

        private void Update()
        {
            if (_cooldownTimer > 0f)
                _cooldownTimer -= Time.deltaTime;

            HandleInput();
        }

        // ─── Private ─────────────────────────────────────────────────────────────
        private void HandleInput()
        {
            if (InputManager.Instance == null)  return;
            if (!InputManager.Instance.AbilityPressed) return;
            if (_currentAbility == null)        return;
            if (IsOnCooldown)
            {
                Debug.Log($"[Ability] {_currentAbility.Name} on cooldown ({CooldownLeft:F1}s).");
                return;
            }

            float cost = _currentAbility.StaminaCost;
            if (_stamina != null && !_stamina.TryConsume(cost))
            {
                Debug.Log("[Ability] Not enough stamina.");
                return;
            }

            _currentAbility.Activate(gameObject);

            float cd = cooldownOverride > 0f ? cooldownOverride : _currentAbility.Cooldown;
            _cooldownTimer = cd;

            OnAbilityActivated?.Invoke(_currentAbility.Name, cd);
        }

        // ─── Public API ──────────────────────────────────────────────────────────

        /// <summary>
        /// Equip a new mutation ability.
        /// Call this from your MinionCreation or Mutation system.
        /// </summary>
        public void SetAbility(IAbility ability)
        {
            _currentAbility = ability ?? new PlaceholderAbility();
            _cooldownTimer  = 0f;
            Debug.Log($"[Ability] Equipped: {_currentAbility.Name}");
        }

        /// <summary>Remove the current ability (resets to placeholder).</summary>
        public void ClearAbility() => SetAbility(null);
    }
}
