using UnityEngine;

namespace MutantMinion.Player
{
    /// <summary>
    /// Handles player movement including walking, sprinting, and jumping with ground detection.
    /// Designed to work with Unity's New Input System through InputManager.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float rotationSpeed = 10f;
        
        [Header("Jump Settings")]
        [SerializeField] private float jumpHeight = 2f;
        [SerializeField] private float gravity = -15f;
        
        [Header("Ground Detection")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundDistance = 0.4f;
        [SerializeField] private LayerMask groundMask;
        
        [Header("Debug")]
        [SerializeField] private bool showJumpDebug = false;

        // Components
        private CharacterController controller;
        private PlayerStamina stamina;
        private Transform cameraTransform;
        
        // Movement state
        private Vector3 velocity;
        private bool isGrounded;
        private bool isSprinting;
        private Vector2 moveInput;
        
        // Properties
        public bool IsMoving => moveInput.magnitude > 0.1f;
        public bool IsSprinting => isSprinting;
        public bool IsGrounded => isGrounded;
        public float CurrentSpeed => isSprinting ? sprintSpeed : walkSpeed;
        
        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            stamina = GetComponent<PlayerStamina>();
        }
        
        private void Start()
        {
            // Cache main camera transform for movement direction calculations
            FindCameraTransform();

            // Create ground check if not assigned
            if (groundCheck == null)
            {
                GameObject groundCheckObj = new GameObject("GroundCheck");
                groundCheckObj.transform.SetParent(transform);

                // Position at the bottom of the CharacterController capsule
                // Bottom = center.y - (height / 2)
                float bottomY = controller.center.y - (controller.height / 2f);
                groundCheckObj.transform.localPosition = new Vector3(0, bottomY, 0);
                groundCheck = groundCheckObj.transform;

                if (showJumpDebug)
                {
                    Debug.Log($"GroundCheck created at local Y: {bottomY} (Controller bottom)");
                }
            }
        }

        private void FindCameraTransform()
        {
            // Try to find main camera
            if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
                return;
            }

            // Fallback: try to find any camera
            Camera camera = FindObjectOfType<Camera>();
            if (camera != null)
            {
                cameraTransform = camera.transform;
                Debug.LogWarning("PlayerMovement: Main camera not found. Using first available camera. Please tag your camera as 'MainCamera'.");
                return;
            }

            Debug.LogError("PlayerMovement: No camera found in scene! Player movement will not work correctly.");
        }
        
        private void Update()
        {
            CheckGroundStatus();
            ApplyGravity();
            HandleMovement();
            ApplyVerticalMovement(); // Apply jump and gravity!
        }
        
        /// <summary>
        /// Called by InputManager when movement input is received
        /// </summary>
        public void OnMove(Vector2 input)
        {
            moveInput = input;
        }
        
        /// <summary>
        /// Called by InputManager when sprint input is received
        /// </summary>
        public void OnSprint(bool sprinting)
        {
            // Only allow sprint if we have stamina
            if (sprinting && stamina != null && !stamina.CanUseSprint())
            {
                isSprinting = false;
                return;
            }
            
            isSprinting = sprinting;
        }
        
        /// <summary>
        /// Called by InputManager when jump input is received
        /// </summary>
        public void OnJump()
        {
            if (showJumpDebug)
            {
                Debug.Log($"Jump Input Received! IsGrounded: {isGrounded}, GroundCheck Pos: {groundCheck?.position}, Velocity.y: {velocity.y}");
            }

            if (isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

                if (showJumpDebug)
                {
                    Debug.Log($"✓ Jump executed! New velocity.y: {velocity.y}");
                }
            }
            else if (showJumpDebug)
            {
                Debug.LogWarning($"✗ Jump failed - Not grounded! Ground Mask: {groundMask.value}, GroundCheck distance: {groundDistance}");
            }
        }
        
        /// <summary>
        /// Boost movement speed - called by PlayerAbility system for mutations
        /// </summary>
        public void ApplySpeedBoost(float multiplier)
        {
            walkSpeed *= multiplier;
            sprintSpeed *= multiplier;
        }
        
        /// <summary>
        /// Boost jump height - called by PlayerAbility system for mutations
        /// </summary>
        public void ApplyJumpBoost(float multiplier)
        {
            jumpHeight *= multiplier;
        }
        
        private void CheckGroundStatus()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            // Debug: Show what the ground check is detecting
            if (showJumpDebug)
            {
                Collider[] hitColliders = Physics.OverlapSphere(groundCheck.position, groundDistance, groundMask);
                Debug.Log($"Ground Check: IsGrounded={isGrounded}, GroundCheck Y={groundCheck.position.y}, " +
                          $"Colliders Found={hitColliders.Length}, Floor Y={FindFloorY()}");

                if (hitColliders.Length > 0)
                {
                    foreach (var col in hitColliders)
                    {
                        Debug.Log($"  - Hit: {col.gameObject.name}, Layer={col.gameObject.layer}");
                    }
                }
            }

            // Reset velocity when grounded
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }
        }

        private float FindFloorY()
        {
            GameObject floor = GameObject.Find("Floor");
            if (floor != null)
            {
                Collider col = floor.GetComponent<Collider>();
                if (col != null)
                    return col.bounds.max.y;
            }
            return 0f;
        }
        
        private void ApplyGravity()
        {
            velocity.y += gravity * Time.deltaTime;
        }

        private void ApplyVerticalMovement()
        {
            // Apply gravity and jump velocity to CharacterController
            controller.Move(velocity * Time.deltaTime);
        }

        private void HandleMovement()
        {
            // Calculate movement direction relative to camera (even if not moving)
            Vector3 direction = GetMovementDirection();

            // Rotate player to face camera forward direction (not movement direction!)
            // This allows strafing and backpedaling while facing forward
            if (cameraTransform != null)
            {
                // Get camera's forward direction (flattened on Y axis)
                Vector3 cameraForward = cameraTransform.forward;
                cameraForward.y = 0f;
                cameraForward.Normalize();

                // Rotate player to face where camera is looking
                if (cameraForward.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
            }

            // Only move if there's input
            if (moveInput.magnitude < 0.1f)
                return;

            // Apply sprint stamina drain
            if (isSprinting && stamina != null)
            {
                stamina.UseSprint(Time.deltaTime);

                // Stop sprinting if out of stamina
                if (!stamina.CanUseSprint())
                {
                    isSprinting = false;
                }
            }

            // Move the player in the calculated direction
            float speed = CurrentSpeed;
            controller.Move(direction * speed * Time.deltaTime);
        }
        
        private Vector3 GetMovementDirection()
        {
            // Get camera forward and right vectors (flattened on Y)
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;
            
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            
            cameraForward.Normalize();
            cameraRight.Normalize();
            
            // Calculate desired move direction based on input
            Vector3 desiredMoveDirection = cameraForward * moveInput.y + cameraRight * moveInput.x;
            desiredMoveDirection.Normalize();
            
            return desiredMoveDirection;
        }
        
        private void OnDrawGizmosSelected()
        {
            // Visualize ground check sphere in editor
            if (groundCheck != null)
            {
                Gizmos.color = isGrounded ? Color.green : Color.red;
                Gizmos.DrawWireSphere(groundCheck.position, groundDistance);

                // Draw a line showing where the ground check is pointing
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * 1f);

                // Show all colliders the ground check can see
                Collider[] hitColliders = Physics.OverlapSphere(groundCheck.position, groundDistance, groundMask);
                if (hitColliders.Length > 0)
                {
                    Gizmos.color = Color.cyan;
                    foreach (var col in hitColliders)
                    {
                        Gizmos.DrawLine(groundCheck.position, col.transform.position);
                    }
                }
            }
        }
    }
}
