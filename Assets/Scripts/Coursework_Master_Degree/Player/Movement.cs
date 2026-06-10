using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;

namespace Coursework_Master_Degree.Player
{
    public class Movement : MonoBehaviour
    {
        // naughty attributes enableif / disableif states
        private bool NotEnabled => false;
        private bool Enabled => true;

        [Header("Components")]
        public Rigidbody PlayerRigidbody;
        public Transform EyesTransform;
        public Camera EyesCamera;

        [Header("Global input system")]
        public InputActionAsset GlobalInputAction;

        [Header("Speed")]
        [Min(0.01f)] public float MoveSpeed;
        [Min(0.01f)] public float SpedUpMoveSpeed;
        [Min(0.01f)] public float SlowDownMoveSpeed;
        [Min(0.01f)] public float CrouchMoveSpeed;

        [Header("Look")]
        [Min(0.01f)] public float LookSpeed;
        [Min(0.01f)] public float UpLimitLookAngle;
        [Min(0.01f)] public float DownLimitLookAngle;

        [Header("Zoom")]
        [Min(0.01f)] public float ZoomLookFieldOfView;
        [EnableIf("NotEnabled")]
        [Min(0.01f)] public float ZoomLookSpeed;
        [Min(0.1f)] public float ZoomLookTransitionDuration;

        [Header("Crouch")]
        [Min(0.3f)] public float CrouchScale;
        [Min(0.1f)] public float CrouchTransitionDuration;

        [EnableIf("NotEnabled")]
        [Tooltip("Exposed to show avaliability, disabled to forbid value set in editor")]
        public float Yaw;
        [EnableIf("NotEnabled")]
        [Tooltip("Exposed to show avaliability, disabled to forbid value set in editor")]
        public float Pitch;

        private InputAction _moveInputAction;
        private InputAction _speedUpInputAction;
        private InputAction _slowDownInputAction;
        private InputAction _lookInputAction;
        private InputAction _crouchInputAction;
        private InputAction _zoomLookInputAction;

        private Vector2 _moveValue;
        private float _speedUpValue;
        private float _slowDownValue;
        private Vector2 _lookValue;
        private float _crouchValue;
        private float _zoomLookValue;

        private float _currentLookSpeed;

        private float _regularLookFieldOfView;
        private bool _isToZoomLook;
        private bool _isZoomLookTransition;
        private float _zoomLookTimePassed;
        private float _zoomLookFrom;
        private float _zoomLookTo;

        private Vector3 _crouchScale;
        private Vector3 _regularScale;
        private bool _isToCrouch;
        private bool _isCrouchTransition;
        private float _crouchTimePassed;
        private Vector3 _scaleFrom;
        private Vector3 _scaleTo;

        // set editor only values
        private void OnValidate()
        {
            // zoom look speed
            if (ZoomLookFieldOfView > 0.0f && LookSpeed > 0.0f)
                ZoomLookSpeed = CalculateZoomLookSpeed();
            else
                ZoomLookSpeed = 0.0f;
        }

        private float CalculateZoomLookSpeed()
        {
            return LookSpeed * ZoomLookFieldOfView / EyesCamera.fieldOfView;
        }

        void Awake()
        {
            HideCursor();

            CalculateZoomLookSpeed();

            _currentLookSpeed = LookSpeed;

            _regularLookFieldOfView = EyesCamera.fieldOfView;

            _crouchScale = new Vector3(1f, CrouchScale, 1f);
            _regularScale = Vector3.one;

            if (UpLimitLookAngle >= DownLimitLookAngle)
            {
                Debug.LogError("UpLimitLookAngle must be smaller than DownLimitLookAngle");
            }

            InputActionMap inputActionMap = GlobalInputAction.FindActionMap("regular_movement", true);
            _moveInputAction = inputActionMap.FindAction("move", true);
            _speedUpInputAction = inputActionMap.FindAction("speed_up", true);
            _slowDownInputAction = inputActionMap.FindAction("slow_down", true);
            _lookInputAction = inputActionMap.FindAction("look", true);
            _zoomLookInputAction = inputActionMap.FindAction("zoom_look", true);
            _crouchInputAction = inputActionMap.FindAction("crouch", true);
        }

        private void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        void OnEnable()
        {
            HideCursor();

            _moveInputAction.Enable();
            _speedUpInputAction.Enable();
            _slowDownInputAction.Enable();
            _lookInputAction.Enable();
            _zoomLookInputAction.Enable();
            _crouchInputAction.Enable();
        }

        void OnDisable()
        {
            _moveInputAction.Disable();
            _speedUpInputAction.Disable();
            _slowDownInputAction.Disable();
            _lookInputAction.Disable();
            _zoomLookInputAction.Disable();
            _crouchInputAction.Disable();
        }

        void Update()
        {
            _moveValue = _moveInputAction.ReadValue<Vector2>();
            _speedUpValue = _speedUpInputAction.ReadValue<float>();
            _slowDownValue = _slowDownInputAction.ReadValue<float>();
            _lookValue = _lookInputAction.ReadValue<Vector2>();
            _zoomLookInputAction.started += 
                (context) =>
                {
                    _zoomLookValue = _zoomLookInputAction.ReadValue<float>();
                };
            _crouchInputAction.started += 
                (context) =>
                {
                    _crouchValue = _crouchInputAction.ReadValue<float>();
                };

            CalculateLook();

            EyesTransform.localRotation = Quaternion.Euler(Pitch, 0f, 0f);

            ApplyZoomLook();
            ApplyCrouch();
        }

        private void CalculateLook()
        {
            Yaw += _lookValue.x * _currentLookSpeed * Time.deltaTime;
            Pitch -= _lookValue.y * _currentLookSpeed * Time.deltaTime;
            Pitch = Mathf.Clamp(Pitch, UpLimitLookAngle, DownLimitLookAngle);
        }

        private void ApplyZoomLook()
        {
            if (_zoomLookValue > 0.5f)
            {
                if (!_isZoomLookTransition)
                {
                    _isToZoomLook = !_isToZoomLook;
                    _isZoomLookTransition = true;

                    if (_isToZoomLook)
                    {
                        _currentLookSpeed = ZoomLookSpeed;

                        _zoomLookFrom = EyesCamera.fieldOfView;
                        _zoomLookTo = ZoomLookFieldOfView;
                    }
                    else
                    {
                        _currentLookSpeed = LookSpeed;

                        _zoomLookFrom = EyesCamera.fieldOfView;
                        _zoomLookTo = _regularLookFieldOfView;
                    }
                }

                _zoomLookValue = default;
            }

            if (_isZoomLookTransition)
            {
                _zoomLookTimePassed += Time.deltaTime / ZoomLookTransitionDuration;
                _zoomLookTimePassed = Mathf.Clamp01(_zoomLookTimePassed);

                EyesCamera.fieldOfView = 
                    Mathf.Lerp(
                        _zoomLookFrom,
                        _zoomLookTo,
                        _zoomLookTimePassed);
                
                if (Mathf.Approximately(
                    EyesCamera.fieldOfView,
                    _zoomLookTo))
                {
                    _isZoomLookTransition = default;
                    _zoomLookTimePassed = default;
                }
            }
        }

        private void ApplyCrouch()
        {
            if (_crouchValue > 0.5f)
            {
                if (!_isCrouchTransition)
                {
                    _isToCrouch = !_isToCrouch;
                    _isCrouchTransition = true;

                    if (_isToCrouch)
                    {
                        _scaleFrom = PlayerRigidbody.transform.localScale;
                        _scaleTo = _crouchScale;
                    }
                    else
                    {
                        _scaleFrom = PlayerRigidbody.transform.localScale;
                        _scaleTo = _regularScale;
                    }
                }

                _crouchValue = default;
            }

            if (_isCrouchTransition)
            {
                _crouchTimePassed += Time.deltaTime / CrouchTransitionDuration;
                _crouchTimePassed = Mathf.Clamp01(_crouchTimePassed);

                PlayerRigidbody.transform.localScale = 
                    Vector3.Lerp(
                        _scaleFrom,
                        _scaleTo,
                        _crouchTimePassed);
                
                if (Mathf.Approximately(
                    PlayerRigidbody.transform.localScale.y,
                    _scaleTo.y))
                {
                    _isCrouchTransition = default;
                    _crouchTimePassed = default;
                }
            }
        }

        void FixedUpdate()
        {
            PlayerRigidbody.MoveRotation(Quaternion.Euler(0f, Yaw, 0f));

            Vector3 moveDirection =
                PlayerRigidbody.transform.forward * _moveValue.y +
                PlayerRigidbody.transform.right * _moveValue.x;
            moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

            float speed = CalculateMoveSpeed();

            Vector3 targetPosition = PlayerRigidbody.position + moveDirection * speed * Time.fixedDeltaTime;

            PlayerRigidbody.MovePosition(targetPosition);
        }

        private float CalculateMoveSpeed()
        {
            if (_isToCrouch)
            {
                return CrouchMoveSpeed;
            }
            else
            {
                if (_slowDownValue > 0.5f)
                {
                    return SlowDownMoveSpeed;
                }
                else if (_speedUpValue > 0.5f)
                {
                    return SpedUpMoveSpeed;
                }
                else
                {
                    return MoveSpeed;
                }
            }
        }
    }
}