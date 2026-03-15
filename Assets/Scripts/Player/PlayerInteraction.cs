// PlayerInteraction.cs
// Detects nearby interactable objects and triggers interaction on F key press.
//
// To make a world object interactable:
//   1. Add the "Interactable" tag to it.
//   2. Optionally implement the IInteractable interface for custom behaviour.
//
// Essential for extraction gameplay:
//   • Loot containers → call Interact() → add to inventory
//   • Extraction points → call Interact() → trigger exit sequence

using System;
using UnityEngine;
using MutantMinion.Input;
using MutantMinion.Utils;

namespace MutantMinion.Player
{
    // ─── Interactable interface ───────────────────────────────────────────────────
    /// <summary>
    /// Implement on any world object that the player can interact with.
    /// Attach the "Interactable" tag so the detection sphere finds it.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>Prompt shown to the player (e.g. "Press F to loot").</summary>
        string InteractPrompt { get; }

        /// <summary>Called when the player presses the interact key.</summary>
        void Interact(GameObject interactor);
    }

    // ─── PlayerInteraction component ─────────────────────────────────────────────
    public class PlayerInteraction : MonoBehaviour
    {
        // ─── Inspector ───────────────────────────────────────────────────────────
        [Header("Detection")]
        [Tooltip("Radius in which the player can detect interactable objects.")]
        [SerializeField] private float detectionRadius = Constants.INTERACTION_RADIUS;

        [Tooltip("Layers to check for interactables.")]
        [SerializeField] private LayerMask interactableMask = ~0; // everything by default

        // ─── State ───────────────────────────────────────────────────────────────
        private IInteractable _nearestInteractable;
        private GameObject    _nearestObject;

        // Buffer to avoid allocating every frame.
        private readonly Collider[] _overlapBuffer = new Collider[16];

        // ─── Events ──────────────────────────────────────────────────────────────
        /// <summary>Fired when the nearest interactable changes (pass null when none).</summary>
        public event Action<IInteractable> OnNearestChanged;

        // ─── Public properties ───────────────────────────────────────────────────
        public IInteractable NearestInteractable => _nearestInteractable;
        public bool          HasInteractable     => _nearestInteractable != null;

        // ─── Unity lifecycle ─────────────────────────────────────────────────────
        private void Update()
        {
            ScanForInteractables();
            HandleInput();
        }

        // ─── Private ─────────────────────────────────────────────────────────────

        private void ScanForInteractables()
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius,
                                                       _overlapBuffer, interactableMask,
                                                       QueryTriggerInteraction.Collide);

            IInteractable best       = null;
            GameObject    bestObject = null;
            float         bestDist   = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                Collider col = _overlapBuffer[i];

                // Skip the player's own collider.
                if (col.gameObject == gameObject) continue;

                // Accept objects tagged Interactable OR that implement IInteractable.
                var interactable = col.GetComponent<IInteractable>();
                if (interactable == null &&
                    !col.CompareTag(Constants.INTERACTABLE_TAG)) continue;

                float dist = Vector3.Distance(transform.position, col.transform.position);
                if (dist < bestDist)
                {
                    bestDist   = dist;
                    best       = interactable;
                    bestObject = col.gameObject;
                }
            }

            // Fire event only when the nearest changes.
            if (bestObject != _nearestObject)
            {
                _nearestInteractable = best;
                _nearestObject       = bestObject;
                OnNearestChanged?.Invoke(_nearestInteractable);

                if (_nearestInteractable != null)
                    Debug.Log($"[Interact] Nearby: {_nearestObject.name} – {_nearestInteractable.InteractPrompt}");
                else
                    Debug.Log("[Interact] No interactable nearby.");
            }
        }

        private void HandleInput()
        {
            if (InputManager.Instance == null)          return;
            if (!InputManager.Instance.InteractPressed) return;

            if (_nearestInteractable != null)
            {
                Debug.Log($"[Interact] Interacting with: {_nearestObject.name}");
                _nearestInteractable.Interact(gameObject);
            }
            else
            {
                Debug.Log("[Interact] Nothing to interact with.");
            }
        }

        // ─── Editor helpers ──────────────────────────────────────────────────────
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
#endif
    }
}
