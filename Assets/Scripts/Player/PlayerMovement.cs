// PlayerMovement.cs
// Handles all locomotion: WASD walking, Shift sprinting, Space jumping,
// ground detection, and gravity.
//
// Mutation hooks (Wolf DNA, Frog DNA, etc.):
//   • SetMoveSpeed(float)  – override move speed (Wolf DNA → speed boost)
//   • SetJumpForce(float)  – override jump force (Frog DNA → higher jump)
//   • Movement direction is always relative to the current camera yaw so the
//     player moves in the direction the camera faces.

using UnityEngine;
using MutantMinion.Input;
using MutantMinion.Utils;

namespace MutantMinion.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        // ─── Inspector ───────────────────────────────────────────────────────────
        [Header("Movement")]
        [Tooltip("Base walking speed in units per second.")]
        [SerializeField] private float moveSpeed   = Constants.DEFAULT_MOVE_SPEED;

        [Tooltip("Speed while sprinting (Shift held).")]
        [SerializeField] private float sprintSpeed = Constants.DEFAULT_SPRINT_SPEED;

        [Tooltip("Smoothing time for acceleration / deceleration.")]
        [SerializeField] private float smoothTime  = Constants.MOVE_SMOOTH_TIME;

        [Header("Jump & Gravity")]
        [Tooltip("Upward velocity applied when jumping.")]
        [SerializeField] private float jumpForce   = Constants.DEFAULT_JUMP_FORCE;

        [Tooltip("Gravity scale. Uses Physics.gravity.y by default.")]
        [SerializeField] private float gravityScale = 1f;

        [Header("Ground Check")]
        [Tooltip("Small sphere radius used to detect the ground beneath the player.")]
        [SerializeField] private float groundCheckRadius   = 0.25f;

        [Tooltip("Transform placed at the base of the player capsule/cube for ground checks.")]
        [SerializeField] private Transform groundCheckPoint;

        [Tooltip("Which layers count as ground.")]
        [SerializeField] private LayerMask groundMask = ~0; // everything by default

        // ─── References ──────────────────────────────────────────────────────────
        private CharacterController _cc;
        private PlayerStamina       _stamina;
        private Camera              _camera;

        // ─── State ───────────────────────────────────────────────────────────────
        private Vector3 _currentHorizontalVelocity;  // current smoothed horizontal velocity
        private Vector3 _smoothDampVelocity;         // internal ref used by SmoothDamp
        private Vector3 _verticalVelocity;            // accumulated gravity / jump
        private bool    _isGrounded;

        // ─── Public read-only state ──────────────────────────────────────────────
        public bool  IsGrounded  => _isGrounded;
        public bool  IsSprinting { get; private set; }
        public float CurrentSpeed => _cc != null ? _cc.velocity.magnitude : 0f;

        // ─── Unity lifecycle ─────────────────────────────────────────────────────
        private void Awake()
        {
            _cc      = GetComponent<CharacterController>();
            _stamina = GetComponent<PlayerStamina>();
            _camera  = Camera.main;

            // If no ground-check point was assigned, use the player's own transform.
            if (groundCheckPoint == null)
                groundCheckPoint = transform;
        }

        private void Update()
        {
            CheckGround();
            HandleGravity();
            HandleMovement();
        }

        // ─── Private ─────────────────────────────────────────────────────────────

        private void CheckGround()
        {
            // Sphere cast slightly below the ground-check point.
            Vector3 origin = groundCheckPoint.position + Vector3.up * 0.1f;
            _isGrounded = Physics.CheckSphere(origin, groundCheckRadius, groundMask,
                                              QueryTriggerInteraction.Ignore);

            // Clear downward velocity when we land so it doesn't accumulate.
            if (_isGrounded && _verticalVelocity.y < 0f)
                _verticalVelocity.y = -2f; // small negative keeps us stuck to slopes
        }

        private void HandleGravity()
        {
            _verticalVelocity.y += Physics.gravity.y * gravityScale * Time.deltaTime;
        }

        private void HandleMovement()
        {
            if (InputManager.Instance == null) return;

            Vector2 raw = InputManager.Instance.MoveInput;

            // ── Jump ──────────────────────────────────────────────────────────
            if (InputManager.Instance.JumpPressed && _isGrounded)
            {
                // v = sqrt(2 * g * h)  – using jumpForce as the upward impulse directly
                _verticalVelocity.y = jumpForce;
            }

            // ── Sprint ────────────────────────────────────────────────────────
            bool wantsToSprint = InputManager.Instance.SprintHeld && raw.sqrMagnitude > 0.01f;
            bool canSprint     = _stamina == null || _stamina.HasStamina(0.01f);

            IsSprinting = wantsToSprint && canSprint;

            if (IsSprinting && _stamina != null)
                _stamina.ConsumePerSecond(Constants.SPRINT_STAMINA_COST);

            // ── Horizontal movement ───────────────────────────────────────────
            float speed = IsSprinting ? sprintSpeed : moveSpeed;

            // Build movement direction relative to camera yaw.
            Vector3 camForward = Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up).normalized;
            Vector3 camRight   = Vector3.ProjectOnPlane(_camera.transform.right,   Vector3.up).normalized;
            Vector3 moveDir    = (camForward * raw.y + camRight * raw.x).normalized;

            // Smooth acceleration/deceleration.
            Vector3 targetVelocity = moveDir * speed;
            _currentHorizontalVelocity = Vector3.SmoothDamp(
                _currentHorizontalVelocity, targetVelocity,
                ref _smoothDampVelocity, smoothTime);

            // Rotate character to face movement direction.
            if (moveDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation  = Quaternion.Slerp(transform.rotation, targetRot, 15f * Time.deltaTime);
            }

            // Combine horizontal and vertical movement.
            Vector3 motion = _currentHorizontalVelocity + _verticalVelocity;
            _cc.Move(motion * Time.deltaTime);
        }

        // ─── Mutation hooks ──────────────────────────────────────────────────────

        /// <summary>Override base move speed (e.g. Wolf DNA speed boost).</summary>
        public void SetMoveSpeed(float newSpeed)   => moveSpeed   = Mathf.Max(0f, newSpeed);

        /// <summary>Override sprint speed (e.g. Wolf DNA sprint boost).</summary>
        public void SetSprintSpeed(float newSpeed) => sprintSpeed = Mathf.Max(0f, newSpeed);

        /// <summary>Override jump force (e.g. Frog DNA higher jump).</summary>
        public void SetJumpForce(float newForce)   => jumpForce   = Mathf.Max(0f, newForce);

        // ─── Editor helpers ──────────────────────────────────────────────────────
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (groundCheckPoint == null) return;
            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Vector3 origin = groundCheckPoint.position + Vector3.up * 0.1f;
            Gizmos.DrawWireSphere(origin, groundCheckRadius);
        }
#endif
    }
}
