using UnityEngine;
using MutantMinion.Player;

namespace MutantMinion.Examples
{
    /// <summary>
    /// Example interactable object demonstrating the IInteractable interface.
    /// Use this as a template for doors, chests, NPCs, etc.
    /// </summary>
    public class ExampleInteractable : MonoBehaviour, IInteractable
    {
        [Header("Interactable Settings")]
        [SerializeField] private string interactionPrompt = "Press F to interact";
        [SerializeField] private bool canInteract = true;
        
        [Header("Visual Feedback")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color highlightColor = Color.yellow;
        
        private Renderer objectRenderer;
        private bool isHighlighted;
        
        private void Awake()
        {
            objectRenderer = GetComponent<Renderer>();
            if (objectRenderer == null)
            {
                objectRenderer = GetComponentInChildren<Renderer>();
            }
        }
        
        public void Interact(GameObject player)
        {
            Debug.Log($"{gameObject.name} was interacted with by {player.name}");
            
            // Example: Toggle interaction state
            canInteract = !canInteract;
            
            // Example: Change color
            if (objectRenderer != null)
            {
                objectRenderer.material.color = canInteract ? normalColor : Color.gray;
            }
        }
        
        public bool CanInteract(GameObject player)
        {
            return canInteract;
        }
        
        public string GetInteractionPrompt()
        {
            return interactionPrompt;
        }
        
        public void OnInteractableEnter(GameObject player)
        {
            isHighlighted = true;
            
            // Visual feedback when player is in range
            if (objectRenderer != null)
            {
                objectRenderer.material.color = highlightColor;
            }
        }
        
        public void OnInteractableExit(GameObject player)
        {
            isHighlighted = false;
            
            // Reset visual feedback
            if (objectRenderer != null)
            {
                objectRenderer.material.color = canInteract ? normalColor : Color.gray;
            }
        }
    }
}
