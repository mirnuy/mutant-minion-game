using UnityEngine;
using MutantMinion.Player;

namespace MutantMinion.Examples
{
    /// <summary>
    /// Example enemy demonstrating the IDamageable interface.
    /// Use this as a template for basic enemy AI and combat.
    /// </summary>
    public class ExampleEnemy : MonoBehaviour, IDamageable
    {
        [Header("Enemy Stats")]
        [SerializeField] private float maxHealth = 50f;
        [SerializeField] private float currentHealth = 50f;

        [Header("AI Settings")]
        [SerializeField] private float followRange = 10f;
        [SerializeField] private float stopDistance = 2f;
        [SerializeField] private float moveSpeed = 2f;

        [Header("Visual Feedback")]
        [SerializeField] private Color normalColor = Color.red;
        [SerializeField] private Color damageColor = Color.white;

        private Renderer enemyRenderer;
        private bool isDead;

        private void Awake()
        {
            enemyRenderer = GetComponent<Renderer>();
            if (enemyRenderer == null)
            {
                enemyRenderer = GetComponentInChildren<Renderer>();
            }
            
            currentHealth = maxHealth;
        }
        
        public void TakeDamage(float damage)
        {
            if (isDead)
                return;
            
            currentHealth -= damage;
            
            Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");
            
            // Visual feedback
            if (enemyRenderer != null)
            {
                StartCoroutine(DamageFlash());
            }
            
            // Check for death
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        private void Die()
        {
            isDead = true;
            Debug.Log($"{gameObject.name} died!");
            
            // Death animation/effects would go here
            
            // For now, just destroy the object
            Destroy(gameObject, 1f);
        }
        
        private System.Collections.IEnumerator DamageFlash()
        {
            if (enemyRenderer != null)
            {
                enemyRenderer.material.color = damageColor;
                yield return new WaitForSeconds(0.1f);
                enemyRenderer.material.color = normalColor;
            }
        }
        
        /// <summary>
        /// Example: Basic AI that follows player on ground plane only
        /// </summary>
        private void Update()
        {
            if (isDead)
                return;

            // Find player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
                return;

            // Calculate distance to player (XZ plane only for proper ground-based following)
            Vector3 playerPos = player.transform.position;
            Vector3 enemyPos = transform.position;

            // Calculate distance on XZ plane (ignore Y to prevent flying/digging)
            Vector3 playerPosFlat = new Vector3(playerPos.x, enemyPos.y, playerPos.z);
            float distance = Vector3.Distance(new Vector3(enemyPos.x, 0, enemyPos.z), 
                                             new Vector3(playerPos.x, 0, playerPos.z));

            // Only follow if player is within follow range AND farther than stop distance
            if (distance <= followRange && distance > stopDistance)
            {
                // Calculate direction on XZ plane only (keeps enemy on ground)
                Vector3 direction = (playerPosFlat - enemyPos).normalized;

                // Move enemy (only affects X and Z, Y stays unchanged)
                transform.position += direction * moveSpeed * Time.deltaTime;

                // Rotate to face player (but only on Y axis)
                Vector3 lookDirection = playerPosFlat - enemyPos;
                if (lookDirection.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
                }
            }
            // If distance > followRange, enemy stops moving (doesn't follow anymore)
        }
    }
}
