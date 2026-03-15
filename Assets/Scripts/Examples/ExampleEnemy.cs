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
        /// Example: Basic AI that follows player
        /// </summary>
        private void Update()
        {
            if (isDead)
                return;
            
            // Simple follow behavior
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                
                // Follow if within range
                if (distance > 2f && distance < 10f)
                {
                    Vector3 direction = (player.transform.position - transform.position).normalized;
                    transform.position += direction * 2f * Time.deltaTime;
                    transform.LookAt(player.transform);
                }
            }
        }
    }
}
