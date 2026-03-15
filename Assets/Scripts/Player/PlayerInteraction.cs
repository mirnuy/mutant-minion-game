using UnityEngine;

namespace MutantMinion.Player
{
    /// <summary>
    /// Handles player interaction with world objects.
    /// Detects interactable objects and triggers interaction events.
    /// </summary>
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private float interactionRange = 2f;
        [SerializeField] private LayerMask interactableMask;
        [SerializeField] private Transform interactionPoint;
        
        [Header("Detection Settings")]
        [SerializeField] private float detectionRadius = 0.5f;
        [SerializeField] private bool showDebugInfo = true;
        
        // Current interactable
        private IInteractable currentInteractable;
        private GameObject currentInteractableObject;
        
        // Properties
        public bool HasInteractable => currentInteractable != null;
        public string InteractablePrompt => currentInteractable?.GetInteractionPrompt() ?? "";
        
        private void Start()
        {
            // Create interaction point if not assigned
            if (interactionPoint == null)
            {
                GameObject interactionPointObj = new GameObject("InteractionPoint");
                interactionPointObj.transform.SetParent(transform);
                interactionPointObj.transform.localPosition = new Vector3(0, 1f, 1f);
                interactionPoint = interactionPointObj.transform;
            }
        }
        
        private void Update()
        {
            DetectInteractables();
        }
        
        /// <summary>
        /// Called by InputManager when interact input is received
        /// </summary>
        public void OnInteract()
        {
            if (currentInteractable != null)
            {
                currentInteractable.Interact(gameObject);
                
                if (showDebugInfo)
                {
                    Debug.Log($"Interacted with: {currentInteractableObject.name}");
                }
            }
        }
        
        private void DetectInteractables()
        {
            // Reset current interactable
            IInteractable previousInteractable = currentInteractable;
            currentInteractable = null;
            currentInteractableObject = null;
            
            // Check for interactables in range
            Collider[] colliders = Physics.OverlapSphere(interactionPoint.position, detectionRadius, interactableMask);
            
            float closestDistance = float.MaxValue;
            
            foreach (Collider col in colliders)
            {
                // Check if object is within interaction range and in front of player
                Vector3 directionToObject = (col.transform.position - transform.position).normalized;
                float distance = Vector3.Distance(transform.position, col.transform.position);
                float dotProduct = Vector3.Dot(transform.forward, directionToObject);
                
                if (distance <= interactionRange && dotProduct > 0.5f)
                {
                    IInteractable interactable = col.GetComponent<IInteractable>();
                    
                    if (interactable != null && interactable.CanInteract(gameObject))
                    {
                        // Get closest interactable
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            currentInteractable = interactable;
                            currentInteractableObject = col.gameObject;
                        }
                    }
                }
            }
            
            // Notify when interactable changes
            if (currentInteractable != previousInteractable)
            {
                // Exit previous interactable
                if (previousInteractable != null)
                {
                    previousInteractable.OnInteractableExit(gameObject);
                }
                
                // Enter new interactable
                if (currentInteractable != null)
                {
                    currentInteractable.OnInteractableEnter(gameObject);
                    
                    if (showDebugInfo)
                    {
                        Debug.Log($"Can interact with: {currentInteractableObject.name}");
                    }
                }
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            if (interactionPoint == null)
                return;
            
            // Draw interaction range
            Gizmos.color = HasInteractable ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(interactionPoint.position, detectionRadius);
            
            // Draw interaction direction
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, transform.forward * interactionRange);
        }
    }
    
    /// <summary>
    /// Interface for interactable objects
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Called when player interacts with this object
        /// </summary>
        void Interact(GameObject player);
        
        /// <summary>
        /// Check if player can interact with this object
        /// </summary>
        bool CanInteract(GameObject player);
        
        /// <summary>
        /// Get the interaction prompt to display to player
        /// </summary>
        string GetInteractionPrompt();
        
        /// <summary>
        /// Called when player enters interaction range
        /// </summary>
        void OnInteractableEnter(GameObject player);
        
        /// <summary>
        /// Called when player exits interaction range
        /// </summary>
        void OnInteractableExit(GameObject player);
    }
}
