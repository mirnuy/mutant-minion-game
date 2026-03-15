// PlayerCamera.cs
// Third-person follow camera.
// The camera orbits around the player using mouse X/Y input, maintaining a
// configurable offset distance and height.  Smooth damping gives a natural feel.
//
// Setup:
//   • Attach to the main Camera GameObject (or a CameraRig parent).
//   • Assign the Player transform in the Inspector.
//   • Mouse cursor is locked/hidden in play mode for FPS-style look.

using UnityEngine;
using MutantMinion.Input;
using MutantMinion.Utils;

namespace MutantMinion.Player
{
    public class PlayerCamera : MonoBehaviour
    {
        // ─── Inspector ───────────────────────────────────────────────────────────
        [Header("Target")]
        [Tooltip("The transform the camera will follow (assign the Player).")]
        [SerializeField] private Transform target;

        [Header("Distance & Height")]
        [Tooltip("Horizontal distance from the player.")]
        [SerializeField] private float distance = Constants.DEFAULT_CAMERA_DISTANCE;

        [Tooltip("Vertical offset above the player pivot.")]
        [SerializeField] private float height   = Constants.DEFAULT_CAMERA_HEIGHT;

        [Header("Sensitivity")]
        [Tooltip("Mouse / right-stick sensitivity multiplier.")]
        [SerializeField] private float sensitivity = Constants.DEFAULT_MOUSE_SENSITIVITY;

        [Header("Pitch Clamp")]
        [Tooltip("Lowest angle (degrees) the camera can tilt down.")]
        [SerializeField] private float minPitch = Constants.CAMERA_MIN_PITCH;

        [Tooltip("Highest angle (degrees) the camera can tilt up.")]
        [SerializeField] private float maxPitch = Constants.CAMERA_MAX_PITCH;

        [Header("Smoothing")]
        [Tooltip("Lower = snappier, higher = more lag.")]
        [SerializeField] private float smoothTime = Constants.CAMERA_SMOOTH_TIME;

        // ─── State ───────────────────────────────────────────────────────────────
        private float _yaw;    // horizontal rotation (around world Y)
        private float _pitch;  // vertical rotation (tilt up/down)
        private Vector3 _smoothCurrentVelocity;

        // ─── Unity lifecycle ─────────────────────────────────────────────────────
        private void Awake()
        {
            // Lock and hide cursor for immersive look input.
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;

            // Initialise rotation from current camera angle so there's no snap on start.
            _yaw   = transform.eulerAngles.y;
            _pitch = transform.eulerAngles.x;
        }

        // LateUpdate runs after all Updates so the camera sees the final position
        // the player moved to this frame.
        private void LateUpdate()
        {
            if (target == null) return;

            HandleRotation();
            FollowTarget();
        }

        // ─── Private ─────────────────────────────────────────────────────────────

        private void HandleRotation()
        {
            if (InputManager.Instance == null) return;

            Vector2 look = InputManager.Instance.LookInput;

            _yaw   += look.x * sensitivity;
            _pitch -= look.y * sensitivity;   // invert Y so moving mouse up looks up
            _pitch  = Mathf.Clamp(_pitch, minPitch, maxPitch);
        }

        private void FollowTarget()
        {
            // Compute desired camera position: start from the target, back off by
            // distance in the direction defined by current yaw/pitch.
            Vector3 pivotPos = target.position + Vector3.up * height;

            Quaternion rotation   = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3   offset      = rotation * new Vector3(0f, 0f, -distance);
            Vector3   desiredPos  = pivotPos + offset;

            // Smooth position to remove jitter.
            transform.position = Vector3.SmoothDamp(transform.position, desiredPos,
                                                     ref _smoothCurrentVelocity, smoothTime);

            // Always look at the pivot point so the player stays centred in view.
            transform.LookAt(pivotPos);
        }

        // ─── Public API ──────────────────────────────────────────────────────────

        /// <summary>
        /// The current horizontal (yaw) angle of the camera in world space.
        /// Used by PlayerMovement to make WASD relative to camera direction.
        /// </summary>
        public float Yaw => _yaw;

        /// <summary>Assign or change the follow target at runtime.</summary>
        public void SetTarget(Transform newTarget) => target = newTarget;

        /// <summary>Change sensitivity at runtime (e.g. from Settings menu).</summary>
        public void SetSensitivity(float newSensitivity) => sensitivity = newSensitivity;
    }
}
