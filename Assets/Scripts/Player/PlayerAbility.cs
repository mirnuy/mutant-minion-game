using UnityEngine;
using System;

namespace MutantMinion.Player
{
    /// <summary>
    /// Manages player mutation abilities (Wolf, Frog, etc.).
    /// Single ability slot system with mutation-specific behaviors.
    /// </summary>
    public class PlayerAbility : MonoBehaviour
    {
        [Header("Ability Settings")]
        [SerializeField] private AbilityData currentAbility;
        [SerializeField] private float abilityCooldown = 5f;
        
        // State
        private bool isAbilityActive;
        private float lastAbilityUseTime;
        private float abilityDuration;
        
        // Components
        private PlayerStamina stamina;
        private PlayerMovement movement;
        private PlayerCombat combat;
        
        // Properties
        public bool HasAbility => currentAbility != null;
        public bool CanUseAbility => HasAbility && !isAbilityActive && Time.time >= lastAbilityUseTime + abilityCooldown;
        public float CooldownRemaining => Mathf.Max(0, (lastAbilityUseTime + abilityCooldown) - Time.time);
        public string CurrentAbilityName => currentAbility?.abilityName ?? "None";
        
        // Events
        public event Action<AbilityData> OnAbilityActivated;
        public event Action<AbilityData> OnAbilityDeactivated;
        public event Action<AbilityData> OnAbilityChanged;
        
        private void Awake()
        {
            stamina = GetComponent<PlayerStamina>();
            movement = GetComponent<PlayerMovement>();
            combat = GetComponent<PlayerCombat>();
        }
        
        /// <summary>
        /// Called by InputManager when ability input is received
        /// </summary>
        public void OnUseAbility()
        {
            if (!CanUseAbility)
                return;
            
            // Check stamina requirement
            if (stamina != null && currentAbility.staminaCost > 0)
            {
                if (!stamina.UseAbilityStamina(currentAbility.staminaCost))
                {
                    Debug.Log("Not enough stamina to use ability!");
                    return;
                }
            }
            
            ActivateAbility();
        }
        
        /// <summary>
        /// Equip a new mutation ability
        /// </summary>
        public void EquipAbility(AbilityData newAbility)
        {
            // Deactivate current ability if active
            if (isAbilityActive)
            {
                DeactivateAbility();
            }
            
            currentAbility = newAbility;
            OnAbilityChanged?.Invoke(currentAbility);
            
            Debug.Log($"Equipped ability: {currentAbility.abilityName}");
        }
        
        /// <summary>
        /// Clear current ability
        /// </summary>
        public void ClearAbility()
        {
            if (isAbilityActive)
            {
                DeactivateAbility();
            }
            
            currentAbility = null;
            OnAbilityChanged?.Invoke(null);
        }
        
        private void ActivateAbility()
        {
            isAbilityActive = true;
            lastAbilityUseTime = Time.time;
            abilityDuration = currentAbility.duration;
            
            // Apply mutation-specific effects
            ApplyAbilityEffects();
            
            OnAbilityActivated?.Invoke(currentAbility);
            
            Debug.Log($"Activated ability: {currentAbility.abilityName}");
            
            // Deactivate after duration (if not permanent)
            if (currentAbility.duration > 0)
            {
                Invoke(nameof(DeactivateAbility), currentAbility.duration);
            }
        }
        
        private void DeactivateAbility()
        {
            if (!isAbilityActive)
                return;
            
            isAbilityActive = false;
            
            // Remove mutation-specific effects
            RemoveAbilityEffects();
            
            OnAbilityDeactivated?.Invoke(currentAbility);
            
            Debug.Log($"Deactivated ability: {currentAbility.abilityName}");
        }
        
        private void ApplyAbilityEffects()
        {
            if (currentAbility == null)
                return;
            
            switch (currentAbility.mutationType)
            {
                case MutationType.Wolf:
                    ApplyWolfEffects();
                    break;
                
                case MutationType.Frog:
                    ApplyFrogEffects();
                    break;
                
                case MutationType.Custom:
                    ApplyCustomEffects();
                    break;
            }
        }
        
        private void RemoveAbilityEffects()
        {
            if (currentAbility == null)
                return;
            
            switch (currentAbility.mutationType)
            {
                case MutationType.Wolf:
                    RemoveWolfEffects();
                    break;
                
                case MutationType.Frog:
                    RemoveFrogEffects();
                    break;
                
                case MutationType.Custom:
                    RemoveCustomEffects();
                    break;
            }
        }
        
        #region Wolf Mutation
        private void ApplyWolfEffects()
        {
            // Wolf: Increased speed and damage
            if (movement != null)
            {
                movement.ApplySpeedBoost(currentAbility.speedMultiplier);
            }
            
            if (combat != null)
            {
                combat.ApplyDamageMultiplier(currentAbility.damageMultiplier);
            }
        }
        
        private void RemoveWolfEffects()
        {
            if (movement != null)
            {
                movement.ApplySpeedBoost(1f / currentAbility.speedMultiplier);
            }
            
            if (combat != null)
            {
                combat.ApplyDamageMultiplier(1f / currentAbility.damageMultiplier);
            }
        }
        #endregion
        
        #region Frog Mutation
        private void ApplyFrogEffects()
        {
            // Frog: Enhanced jump height
            if (movement != null)
            {
                movement.ApplyJumpBoost(currentAbility.jumpMultiplier);
            }
        }
        
        private void RemoveFrogEffects()
        {
            if (movement != null)
            {
                movement.ApplyJumpBoost(1f / currentAbility.jumpMultiplier);
            }
        }
        #endregion
        
        #region Custom Mutation (Extensible)
        private void ApplyCustomEffects()
        {
            // Override in derived classes or implement custom logic
            Debug.Log("Custom mutation effects applied");
        }
        
        private void RemoveCustomEffects()
        {
            // Override in derived classes or implement custom logic
            Debug.Log("Custom mutation effects removed");
        }
        #endregion
    }
    
    /// <summary>
    /// Mutation types available in the game
    /// </summary>
    public enum MutationType
    {
        None,
        Wolf,   // Speed and damage boost
        Frog,   // Jump boost
        Custom  // For future mutations
    }
    
    /// <summary>
    /// Scriptable Object data for abilities
    /// </summary>
    [CreateAssetMenu(fileName = "NewAbility", menuName = "Mutant Minion/Ability Data")]
    public class AbilityData : ScriptableObject
    {
        [Header("Basic Info")]
        public string abilityName = "New Ability";
        public string description = "Ability description";
        public MutationType mutationType = MutationType.None;
        
        [Header("Ability Stats")]
        public float staminaCost = 20f;
        public float duration = 5f; // 0 for instant abilities
        public float cooldown = 5f;
        
        [Header("Mutation Effects")]
        public float speedMultiplier = 1f;
        public float jumpMultiplier = 1f;
        public float damageMultiplier = 1f;
    }
}
