using UnityEngine;
using System;

namespace MutantMinion.Player
{
    /// <summary>
    /// Manages player stamina for sprinting, abilities, and future features.
    /// Handles stamina consumption, regeneration, and depletion states.
    /// </summary>
    public class PlayerStamina : MonoBehaviour
    {
        [Header("Stamina Settings")]
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float currentStamina = 100f;
        
        [Header("Sprint Settings")]
        [SerializeField] private float sprintDrainRate = 10f;
        [SerializeField] private float minStaminaForSprint = 10f;
        
        [Header("Regeneration")]
        [SerializeField] private float staminaRegenRate = 15f;
        [SerializeField] private float regenDelayAfterUse = 1f;
        
        // State
        private float lastStaminaUseTime;
        private bool isRegenerating;
        
        // Properties
        public float CurrentStamina => currentStamina;
        public float MaxStamina => maxStamina;
        public float StaminaPercentage => currentStamina / maxStamina;
        public bool IsDepleted => currentStamina <= 0f;
        
        // Events for UI updates
        public event Action<float, float> OnStaminaChanged;
        
        private void Start()
        {
            currentStamina = maxStamina;
        }
        
        private void Update()
        {
            HandleStaminaRegeneration();
        }
        
        /// <summary>
        /// Use stamina for sprinting
        /// </summary>
        public void UseSprint(float deltaTime)
        {
            UseStamina(sprintDrainRate * deltaTime);
        }
        
        /// <summary>
        /// Check if player has enough stamina to sprint
        /// </summary>
        public bool CanUseSprint()
        {
            return currentStamina >= minStaminaForSprint;
        }
        
        /// <summary>
        /// Use stamina for abilities
        /// </summary>
        public bool UseAbilityStamina(float amount)
        {
            if (currentStamina >= amount)
            {
                UseStamina(amount);
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Restore stamina (for pickups or abilities)
        /// </summary>
        public void RestoreStamina(float amount)
        {
            currentStamina = Mathf.Min(currentStamina + amount, maxStamina);
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        }
        
        /// <summary>
        /// Increase max stamina (for upgrades/mutations)
        /// </summary>
        public void IncreaseMaxStamina(float amount)
        {
            maxStamina += amount;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        }
        
        /// <summary>
        /// Modify stamina regeneration rate (for mutations)
        /// </summary>
        public void ModifyRegenRate(float multiplier)
        {
            staminaRegenRate *= multiplier;
        }
        
        private void UseStamina(float amount)
        {
            currentStamina = Mathf.Max(currentStamina - amount, 0f);
            lastStaminaUseTime = Time.time;
            isRegenerating = false;
            
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        }
        
        private void HandleStaminaRegeneration()
        {
            // Check if enough time has passed since last stamina use
            if (Time.time - lastStaminaUseTime < regenDelayAfterUse)
                return;
            
            // Regenerate stamina if not at max
            if (currentStamina < maxStamina)
            {
                isRegenerating = true;
                currentStamina = Mathf.Min(currentStamina + staminaRegenRate * Time.deltaTime, maxStamina);
                OnStaminaChanged?.Invoke(currentStamina, maxStamina);
            }
            else
            {
                isRegenerating = false;
            }
        }
        
        /// <summary>
        /// Get stamina info for debugging
        /// </summary>
        public string GetStaminaInfo()
        {
            return $"Stamina: {currentStamina:F1}/{maxStamina:F1} ({StaminaPercentage * 100:F0}%)";
        }
    }
}
