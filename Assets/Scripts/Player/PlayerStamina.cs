// PlayerStamina.cs
// Manages the stamina resource used for sprinting and abilities.
// Regen is delayed after the last stamina expenditure so the system
// feels tactical rather than instant-refilling.
//
// Mutation hooks:
//   • Troll DNA → increase regenRate or decrease costs
//   • Any DNA   → modify maxStamina

using UnityEngine;
using MutantMinion.Utils;

namespace MutantMinion.Player
{
    public class PlayerStamina : MonoBehaviour
    {
        // ─── Inspector ───────────────────────────────────────────────────────────
        [Header("Stamina Settings")]
        [Tooltip("Maximum stamina pool.")]
        [SerializeField] private float maxStamina = Constants.DEFAULT_MAX_STAMINA;

        [Tooltip("Stamina recovered per second when regen is active.")]
        [SerializeField] private float regenRate = Constants.DEFAULT_STAMINA_REGEN_RATE;

        [Tooltip("Seconds after last stamina use before regen begins.")]
        [SerializeField] private float regenDelay = Constants.DEFAULT_STAMINA_REGEN_DELAY;

        // ─── State ───────────────────────────────────────────────────────────────
        private float currentStamina;
        private float regenTimer;

        // ─── Public API ──────────────────────────────────────────────────────────
        /// <summary>Current stamina value, clamped to [0, maxStamina].</summary>
        public float Current => currentStamina;

        /// <summary>Stamina as a 0–1 fraction (useful for UI progress bars).</summary>
        public float Fraction => currentStamina / maxStamina;

        /// <summary>Maximum stamina (can be modified by mutations).</summary>
        public float Max => maxStamina;

        // ─── Unity lifecycle ─────────────────────────────────────────────────────
        private void Awake()
        {
            currentStamina = maxStamina;
        }

        private void Update()
        {
            HandleRegen();
        }

        // ─── Private ─────────────────────────────────────────────────────────────
        private void HandleRegen()
        {
            if (currentStamina >= maxStamina) return;

            regenTimer -= Time.deltaTime;
            if (regenTimer <= 0f)
            {
                currentStamina = Mathf.Min(currentStamina + regenRate * Time.deltaTime, maxStamina);
            }
        }

        // ─── Public methods ──────────────────────────────────────────────────────

        /// <summary>
        /// Attempt to consume <paramref name="amount"/> stamina.
        /// Returns true if successful, false if there was not enough stamina.
        /// </summary>
        public bool TryConsume(float amount)
        {
            if (currentStamina < amount) return false;

            currentStamina -= amount;
            regenTimer = regenDelay;   // reset the delay each time stamina is used
            return true;
        }

        /// <summary>
        /// Consume stamina continuously (e.g. while sprinting).
        /// Call this every frame with <c>amount * Time.deltaTime</c> already applied,
        /// or pass the per-second rate and let this method scale it.
        /// </summary>
        public bool ConsumePerSecond(float ratePerSecond)
        {
            return TryConsume(ratePerSecond * Time.deltaTime);
        }

        /// <summary>True when there is enough stamina for the given cost.</summary>
        public bool HasStamina(float amount) => currentStamina >= amount;

        /// <summary>
        /// Directly set max stamina – called by mutation system.
        /// (e.g. Troll DNA could increase the pool)
        /// </summary>
        public void SetMaxStamina(float newMax)
        {
            maxStamina     = Mathf.Max(1f, newMax);
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }

        /// <summary>
        /// Directly set regen rate – called by mutation system.
        /// (e.g. Troll DNA could accelerate regen)
        /// </summary>
        public void SetRegenRate(float newRate)
        {
            regenRate = Mathf.Max(0f, newRate);
        }
    }
}
