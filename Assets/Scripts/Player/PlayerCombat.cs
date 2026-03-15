// PlayerCombat.cs
// Handles attack input, cooldowns, and hit detection.
//
// Current implementation:
//   • Left Mouse Button → Light Attack (raycast hit, debug output)
//   • Right Mouse Button → Block (reserved – not yet implemented)
//
// Future additions:
//   • Block / parry on Right Mouse
//   • Heavy attack when a dedicated button is chosen
//   • Animation events to trigger hit detection at the right moment
//
// Mutation hooks (Spider DNA, etc.):
//   • SetAttackDamage(float) – increase damage from outside
//   • OnAttackHit event      – mutations can subscribe to apply poison, etc.

using System;
using UnityEngine;
using MutantMinion.Input;
using MutantMinion.Utils;

namespace MutantMinion.Player
{
    public class PlayerCombat : MonoBehaviour
    {
        // ─── Inspector ───────────────────────────────────────────────────────────
        [Header("Light Attack")]
        [Tooltip("Damage dealt per light attack.")]
        [SerializeField] private float lightAttackDamage  = Constants.LIGHT_ATTACK_DAMAGE;

        [Tooltip("Reach of the light-attack raycast in units.")]
        [SerializeField] private float lightAttackRange   = Constants.LIGHT_ATTACK_RANGE;

        [Tooltip("Minimum seconds between consecutive light attacks.")]
        [SerializeField] private float lightAttackCooldown = Constants.LIGHT_ATTACK_COOLDOWN;

        [Header("Hit Detection")]
        [Tooltip("Origin of the attack ray (defaults to main Camera if not assigned).")]
        [SerializeField] private Transform attackOrigin;

        [Tooltip("Layers that can be hit by attacks.")]
        [SerializeField] private LayerMask hitMask = ~0;

        // ─── State ───────────────────────────────────────────────────────────────
        private float _lightAttackTimer;
        private bool  _isAttacking;

        // ─── Events (subscribe from mutation scripts) ─────────────────────────────
        /// <summary>Fired when a light attack successfully hits something.</summary>
        public event Action<GameObject, float> OnAttackHit;

        // ─── Unity lifecycle ─────────────────────────────────────────────────────
        private void Awake()
        {
            if (attackOrigin == null)
            {
                // Fall back to the main camera so the ray comes from the crosshair.
                var cam = Camera.main;
                if (cam != null) attackOrigin = cam.transform;
            }
        }

        private void Update()
        {
            // Count down cooldown timers.
            if (_lightAttackTimer > 0f)
                _lightAttackTimer -= Time.deltaTime;

            HandleInput();
        }

        // ─── Private ─────────────────────────────────────────────────────────────

        private void HandleInput()
        {
            if (InputManager.Instance == null) return;

            // ── Light Attack ──────────────────────────────────────────────────
            if (InputManager.Instance.LightAttackPressed && _lightAttackTimer <= 0f)
            {
                PerformLightAttack();
            }

            // ── Block (reserved) ──────────────────────────────────────────────
            // Right-click input is captured by InputManager but not acted on yet.
            // Add block/parry logic here when the mechanic is designed.
        }

        private void PerformLightAttack()
        {
            _lightAttackTimer = lightAttackCooldown;
            _isAttacking      = true;

            // In a full implementation, you'd trigger an animation here:
            // _animator.SetTrigger("LightAttack");
            // Hit detection would then fire from an animation event.
            // For MVP, we do the raycast immediately.

            DetectHit(lightAttackDamage);

            _isAttacking = false;
        }

        private void DetectHit(float damage)
        {
            if (attackOrigin == null) return;

            Ray ray = new Ray(attackOrigin.position, attackOrigin.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, lightAttackRange, hitMask,
                                QueryTriggerInteraction.Ignore))
            {
                Debug.Log($"[Combat] Light Attack hit: {hit.collider.gameObject.name} " +
                          $"for {damage} damage at {hit.point}");

                // Notify any damageable component on the target.
                // Replace with your own IDamageable interface when implemented.
                // hit.collider.GetComponent<IDamageable>()?.TakeDamage(damage);

                // Fire event so mutation scripts can react (e.g. Spider → apply poison).
                OnAttackHit?.Invoke(hit.collider.gameObject, damage);
            }
        }

        // ─── Public API ──────────────────────────────────────────────────────────

        /// <summary>True while an attack animation / action is in progress.</summary>
        public bool IsAttacking => _isAttacking;

        /// <summary>
        /// Override attack damage – called by mutation system.
        /// (e.g. equipping a weapon or Spider DNA poison stacks)
        /// </summary>
        public void SetAttackDamage(float newDamage) => lightAttackDamage = Mathf.Max(0f, newDamage);

        /// <summary>Override attack range – useful for weapon types.</summary>
        public void SetAttackRange(float newRange) => lightAttackRange = Mathf.Max(0f, newRange);
    }
}
