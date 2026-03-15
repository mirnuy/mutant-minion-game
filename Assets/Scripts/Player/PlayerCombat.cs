using UnityEngine;
using System.Collections;

namespace MutantMinion.Player
{
    /// <summary>
    /// Handles player combat including light attack and block (reserved for future).
    /// Designed to work with mutation system for enhanced combat abilities.
    /// </summary>
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Attack Settings")]
        [SerializeField] private float lightAttackDamage = 10f;
        [SerializeField] private float lightAttackRange = 2f;
        [SerializeField] private float lightAttackCooldown = 0.5f;
        [SerializeField] private LayerMask enemyLayer;
        
        [Header("Attack Detection")]
        [SerializeField] private Transform attackPoint;
        [SerializeField] private float attackRadius = 1f;
        
        [Header("Block Settings (Reserved)")]
        [SerializeField] private bool blockEnabled = false; // For future implementation
        
        // State
        private bool isAttacking;
        private float lastAttackTime;
        private bool isBlocking;
        
        // Components
        private PlayerStamina stamina;
        private Animator animator; // Ready for animation integration
        
        // Properties
        public bool IsAttacking => isAttacking;
        public bool IsBlocking => isBlocking;
        public bool CanAttack => !isAttacking && Time.time >= lastAttackTime + lightAttackCooldown;
        
        // Damage multiplier for mutations
        private float damageMultiplier = 1f;
        
        private void Awake()
        {
            stamina = GetComponent<PlayerStamina>();
            animator = GetComponent<Animator>();
        }
        
        private void Start()
        {
            // Create attack point if not assigned
            if (attackPoint == null)
            {
                GameObject attackPointObj = new GameObject("AttackPoint");
                attackPointObj.transform.SetParent(transform);
                attackPointObj.transform.localPosition = new Vector3(0, 1f, 1f);
                attackPoint = attackPointObj.transform;
            }
        }
        
        /// <summary>
        /// Called by InputManager when light attack input is received
        /// </summary>
        public void OnLightAttack()
        {
            if (!CanAttack)
                return;
            
            PerformLightAttack();
        }
        
        /// <summary>
        /// Called by InputManager when block input is received (reserved for future)
        /// </summary>
        public void OnBlock(bool blocking)
        {
            if (!blockEnabled)
                return;
            
            // TODO: Implement blocking system
            isBlocking = blocking;
            
            // Future: Reduce damage, drain stamina, etc.
        }
        
        /// <summary>
        /// Apply damage multiplier - called by PlayerAbility system for mutations
        /// </summary>
        public void ApplyDamageMultiplier(float multiplier)
        {
            damageMultiplier = multiplier;
        }
        
        private void PerformLightAttack()
        {
            isAttacking = true;
            lastAttackTime = Time.time;
            
            // Trigger animation when ready
            if (animator != null)
            {
                animator.SetTrigger("LightAttack");
            }
            
            // Detect enemies in attack range
            Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRadius, enemyLayer);
            
            foreach (Collider enemy in hitEnemies)
            {
                // Check if enemy is in front of player
                Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
                float dotProduct = Vector3.Dot(transform.forward, directionToEnemy);
                
                if (dotProduct > 0.5f) // Enemy is in front
                {
                    // Calculate final damage with multiplier
                    float finalDamage = lightAttackDamage * damageMultiplier;
                    
                    // Try to damage the enemy
                    IDamageable damageable = enemy.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(finalDamage);
                    }
                }
            }
            
            // Visual/audio feedback
            Debug.Log($"Light Attack! Damage: {lightAttackDamage * damageMultiplier}");
            
            // Reset attack state after cooldown
            StartCoroutine(ResetAttackState());
        }
        
        private IEnumerator ResetAttackState()
        {
            yield return new WaitForSeconds(0.3f);
            isAttacking = false;
        }
        
        private void OnDrawGizmosSelected()
        {
            if (attackPoint == null)
                return;
            
            // Visualize attack range
            Gizmos.color = isAttacking ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
            
            // Draw attack direction
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * lightAttackRange);
        }
    }
    
    /// <summary>
    /// Interface for damageable entities
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(float damage);
    }
}
