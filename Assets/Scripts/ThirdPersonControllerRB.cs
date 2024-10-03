using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace StarterAssets
{
    [RequireComponent(typeof(Rigidbody))]
    public class ThirdPersonControllerRB : MonoBehaviour
    {
        public bool disableMovement = false;

        [Header("Player")]
        public float MoveSpeed = 2.0f;
        public float SprintSpeed = 5.335f;
        [Range(0.0f, 0.3f)] public float RotationSmoothTime = 0.12f;
        public float SpeedChangeRate = 10.0f;

        [Space(10)] public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;

        [Space(10)] public float JumpTimeout = 0.50f;
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.28f;
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        public GameObject CinemachineCameraTarget;
        public float TopClamp = 70.0f;
        public float BottomClamp = -30.0f;
        public float CameraAngleOverride = 0.0f;
        public bool LockCameraPosition = false;

        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;

        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        private Animator _animator;
        private Rigidbody _rigidbody;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;
        private bool _hasAnimator;

        [Header("Dash")]
        public float DashSpeed = 10.0f; // New: Dash speed
        public float DashDuration = 0.5f; // New: Duration of the dash in seconds
        public float DashCooldown = 2.0f; // New: Cooldown between dashes
        // New variables for dash state
        private bool _isDashing = false;
        private float _dashTimeLeft;
        private float _dashCooldownLeft;
        private Vector3 _dashDirection;

        public void StopMovement()
        {
            _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, 0);
                _animator.SetFloat("MoveX", 0);
                _animator.SetFloat("MoveY", 0);
            }
        }
        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.freezeRotation = true; // Freeze rotation so character doesn't tip over.

            AssignAnimationIDs();

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);
            // Handle dashing
            HandleDash();
            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // Get input for camera look direction
            float lookX = Input.GetAxis("Mouse X");
            float lookY = Input.GetAxis("Mouse Y");

            // If there's input and camera position isn't locked
            if ((lookX != 0 || lookY != 0) && !LockCameraPosition)
            {
                // Don't multiply mouse input by Time.deltaTime
                _cinemachineTargetYaw += lookX;
                _cinemachineTargetPitch -= lookY;
            }

            // Clamp the pitch
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);

            CinemachineCameraTarget.transform.parent = null;
            _targetRotation = _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            CinemachineCameraTarget.transform.parent = transform;
        }

        private void Move()
        {
            if (disableMovement || _isDashing)
                return;

            // Set target speed based on move speed, sprint speed, and if sprint is pressed
            //float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? SprintSpeed : MoveSpeed;
            float targetSpeed = SprintSpeed;
            if (GameManager.Instance.GetHackMode())
            {
                targetSpeed = MoveSpeed;
            }

            // If there's no input or movement is disabled, set target speed to 0
            float horizontal = 0;
            float vertical = 0;
            if (Input.GetKey(KeyCode.W))
            {
                vertical = 1;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                vertical = -1;
            }

            if (Input.GetKey(KeyCode.D))
            {
                horizontal = 1;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                horizontal = -1;
            }

            if (horizontal == 0 && vertical == 0)
            {
                targetSpeed = 0.0f;
            }

            // Get the current horizontal speed ignoring vertical velocity
            float currentHorizontalSpeed = new Vector3(_rigidbody.velocity.x, 0.0f, _rigidbody.velocity.z).magnitude;

            float speedOffset = 0.1f;

            // Accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            // Update the animation blend value
            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // Calculate movement direction based on camera's forward direction
            Vector3 targetDirection = _mainCamera.transform.forward * vertical + _mainCamera.transform.right * horizontal;
            targetDirection.y = 0.0f; // Ignore the vertical component to stay on the ground

            // Apply movement to the rigidbody
            _rigidbody.velocity = new Vector3(targetDirection.normalized.x * _speed, _rigidbody.velocity.y, targetDirection.normalized.z * _speed);

            // Update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat("MoveX", horizontal);
                _animator.SetFloat("MoveY", vertical);
            }
        }

        public void Move(Vector3 dir,float speed)
        {
            // Set target speed based on move speed, sprint speed, and if sprint is pressed
            float targetSpeed = speed;

            // Get the current horizontal speed ignoring vertical velocity
            float currentHorizontalSpeed = new Vector3(_rigidbody.velocity.x, 0.0f, _rigidbody.velocity.z).magnitude;

            float speedOffset = 0.1f;

            // Accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            // Update the animation blend value
            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // Calculate movement direction based on camera's forward direction
            Vector3 targetDirection = dir;
            targetDirection.y = 0.0f; // Ignore the vertical component to stay on the ground

            // Apply movement to the rigidbody
            _rigidbody.velocity = new Vector3(targetDirection.normalized.x * _speed, _rigidbody.velocity.y, targetDirection.normalized.z * _speed);

            // Update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;

                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (Input.GetButtonDown("Jump") && !disableMovement && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _verticalVelocity, _rigidbody.velocity.z);

                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // Jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // Apply gravity
                _verticalVelocity += Gravity * Time.deltaTime;
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _verticalVelocity, _rigidbody.velocity.z);
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }

        private void HandleDash()
        {
            if (disableMovement)
            {
                if (_isDashing)
                {
                    EndDash();
                }
                return;
            }
            // Check for dash input and ensure dash is not on cooldown
            if (Input.GetKeyDown(KeyCode.LeftShift) && _dashCooldownLeft <= 0.0f && !_isDashing)
            {
                StartDash();
            }

            // If dashing, handle dash movement and cooldown timer
            if (_isDashing)
            {
                if (_dashTimeLeft > 0)
                {
                    _dashTimeLeft -= Time.deltaTime;
                    _rigidbody.velocity = _dashDirection * DashSpeed; // Maintain the same direction for the duration of the dash
                }
                else
                {
                    EndDash();
                }
            }

            // Handle cooldown
            if (_dashCooldownLeft > 0)
            {
                _dashCooldownLeft -= Time.deltaTime;
            }
        }

        private void StartDash()
        {
            _isDashing = true;
            _dashTimeLeft = DashDuration;
            _dashCooldownLeft = DashCooldown;

            // Set the dash direction to the current movement direction
            _dashDirection = new Vector3(_rigidbody.velocity.x, 0.0f, _rigidbody.velocity.z).normalized;
            if (_dashDirection.magnitude == 0)
            {
                // If no movement input, dash forward
                _dashDirection = transform.forward;
            }
        }

        private void EndDash()
        {
            _isDashing = false;
        }
    }
}
