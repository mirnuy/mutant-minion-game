using UnityEngine;

namespace MutantMinion.Player
{
    /// <summary>
    /// Third-person camera controller with mouse look functionality.
    /// Handles camera rotation, zoom, and collision detection.
    /// </summary>
    public class PlayerCamera : MonoBehaviour
    {
        [Header("Camera Target")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 targetOffset = new Vector3(0, 1.5f, 0);
        
        [Header("Camera Settings")]
        [SerializeField] private float distance = 5f;
        [SerializeField] private float minDistance = 2f;
        [SerializeField] private float maxDistance = 10f;
        
        [Header("Rotation Settings")]
        [SerializeField] private float mouseSensitivity = 1f; // Reduced from 2f
        [SerializeField] private float gamepadSensitivity = 150f; // For gamepad (degrees per second)
        [SerializeField] private float minVerticalAngle = -40f;
        [SerializeField] private float maxVerticalAngle = 70f;
        [SerializeField] private float rotationSmoothTime = 0.1f;

        [Header("Sensitivity Ranges (for UI sliders)")]
        [SerializeField] private float minSensitivity = 0.1f;
        [SerializeField] private float maxSensitivity = 5f;
        
        [Header("Collision Settings")]
        [SerializeField] private bool checkCollision = true;
        [SerializeField] private float collisionOffset = 0.2f;
        [SerializeField] private LayerMask collisionMask = -1;
        [SerializeField] private float collisionSmoothTime = 0.1f;
        [SerializeField] private float cameraRadius = 0.3f; // Sphere cast radius
        
        // Camera rotation
        private float rotationX;
        private float rotationY;
        private float currentRotationX;
        private float currentRotationY;
        private float rotationXVelocity;
        private float rotationYVelocity;

        // Camera zoom
        private float currentDistance;
        private float targetDistance;
        private float collisionDistanceVelocity; // For smooth collision adjustment

        // Input
        private Vector2 lookInput;
        
        private void Start()
        {
            // Initialize camera position
            if (target == null)
            {
                Debug.LogError("PlayerCamera: Target not assigned! Please assign the player transform.");
                enabled = false;
                return;
            }
            
            currentDistance = distance;
            targetDistance = distance;
            
            // Initialize rotation to look at target
            Vector3 angles = transform.eulerAngles;
            rotationX = angles.y;
            rotationY = angles.x;
            
            // Lock cursor for gameplay
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        private void LateUpdate()
        {
            if (target == null)
                return;
            
            UpdateCameraRotation();
            UpdateCameraPosition();
        }
        
        /// <summary>
        /// Called by InputManager when look input is received
        /// </summary>
        public void OnLook(Vector2 input)
        {
            lookInput = input;
        }
        
        /// <summary>
        /// Called by InputManager when zoom input is received
        /// </summary>
        public void OnZoom(float zoomDelta)
        {
            targetDistance = Mathf.Clamp(targetDistance - zoomDelta, minDistance, maxDistance);
        }
        
        /// <summary>
        /// Toggle cursor lock state (for pause menus, etc.)
        /// </summary>
        public void ToggleCursorLock(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }

        /// <summary>
        /// Set mouse sensitivity (for UI settings)
        /// </summary>
        /// <param name="sensitivity">Sensitivity value (will be clamped to min/max range)</param>
        public void SetMouseSensitivity(float sensitivity)
        {
            mouseSensitivity = Mathf.Clamp(sensitivity, minSensitivity, maxSensitivity);
        }

        /// <summary>
        /// Get current mouse sensitivity
        /// </summary>
        public float GetMouseSensitivity()
        {
            return mouseSensitivity;
        }

        /// <summary>
        /// Get sensitivity as normalized value (0-1) for UI sliders
        /// </summary>
        public float GetNormalizedSensitivity()
        {
            return Mathf.InverseLerp(minSensitivity, maxSensitivity, mouseSensitivity);
        }

        /// <summary>
        /// Set sensitivity from normalized value (0-1) for UI sliders
        /// </summary>
        public void SetNormalizedSensitivity(float normalizedValue)
        {
            mouseSensitivity = Mathf.Lerp(minSensitivity, maxSensitivity, normalizedValue);
        }

        /// <summary>
        /// Get sensitivity range for UI (min, max)
        /// </summary>
        public Vector2 GetSensitivityRange()
        {
            return new Vector2(minSensitivity, maxSensitivity);
        }
        
        private void UpdateCameraRotation()
        {
            // Apply mouse input to rotation
            rotationX += lookInput.x * mouseSensitivity;
            rotationY -= lookInput.y * mouseSensitivity;
            
            // Clamp vertical rotation
            rotationY = Mathf.Clamp(rotationY, minVerticalAngle, maxVerticalAngle);
            
            // Smooth rotation
            currentRotationX = Mathf.SmoothDampAngle(currentRotationX, rotationX, ref rotationXVelocity, rotationSmoothTime);
            currentRotationY = Mathf.SmoothDampAngle(currentRotationY, rotationY, ref rotationYVelocity, rotationSmoothTime);
        }
        
        private void UpdateCameraPosition()
        {
            // Calculate target position (player position + offset)
            Vector3 targetPosition = target.position + targetOffset;

            // Calculate desired camera rotation
            Quaternion rotation = Quaternion.Euler(currentRotationY, currentRotationX, 0);

            // Calculate desired camera direction
            Vector3 direction = rotation * -Vector3.forward;

            // Determine final distance (with collision)
            float finalDistance = currentDistance;

            if (checkCollision)
            {
                finalDistance = GetCollisionSafeDistance(targetPosition, direction, currentDistance);
            }

            // Smoothly adjust distance
            currentDistance = Mathf.SmoothDamp(currentDistance, finalDistance, ref collisionDistanceVelocity, collisionSmoothTime);

            // Calculate final camera position
            Vector3 finalPosition = targetPosition + direction * currentDistance;

            // Apply position
            transform.position = finalPosition;

            // Always look at target
            transform.LookAt(targetPosition);
        }

        /// <summary>
        /// Get safe camera distance using SphereCast to prevent clipping
        /// </summary>
        private float GetCollisionSafeDistance(Vector3 targetPosition, Vector3 direction, float desiredDistance)
        {
            float safeDistance = desiredDistance;

            // Use SphereCast for better collision detection (prevents clipping)
            RaycastHit hit;
            if (Physics.SphereCast(targetPosition, cameraRadius, direction, out hit, desiredDistance, collisionMask))
            {
                // Calculate distance to collision point, accounting for camera radius
                safeDistance = Mathf.Clamp(hit.distance - collisionOffset, minDistance, desiredDistance);
            }

            // Additional ground check when looking down
            if (currentRotationY < -10f) // Looking down
            {
                // Check if camera would go below ground
                RaycastHit groundHit;
                Vector3 potentialPosition = targetPosition + direction * safeDistance;

                if (Physics.Raycast(potentialPosition, Vector3.down, out groundHit, 2f, collisionMask))
                {
                    // If we'd be too close to ground, adjust distance
                    if (groundHit.distance < cameraRadius + collisionOffset)
                    {
                        // Move camera up slightly to avoid ground clipping
                        float groundAdjustment = (cameraRadius + collisionOffset - groundHit.distance);
                        safeDistance = Mathf.Max(safeDistance - groundAdjustment, minDistance);
                    }
                }
            }

            // Smoothly return to target distance when clear
            targetDistance = Mathf.Lerp(targetDistance, distance, Time.deltaTime * 2f);

            return Mathf.Min(safeDistance, targetDistance);
        }
        
        private void OnDrawGizmosSelected()
        {
            if (target == null)
                return;

            // Draw camera target position
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(target.position + targetOffset, 0.2f);

            // Draw camera collision check
            if (checkCollision && Application.isPlaying)
            {
                Vector3 targetPos = target.position + targetOffset;

                // Draw line from target to camera
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(targetPos, transform.position);

                // Draw camera sphere (shows collision radius)
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, cameraRadius);

                // Draw desired position (without collision)
                Quaternion rotation = Quaternion.Euler(currentRotationY, currentRotationX, 0);
                Vector3 desiredPos = targetPos + (rotation * -Vector3.forward) * distance;
                Gizmos.color = Color.gray;
                Gizmos.DrawWireSphere(desiredPos, 0.1f);
                Gizmos.DrawLine(targetPos, desiredPos);
            }
        }
    }
}
