using UnityEngine;
using UnityEngine.InputSystem;

namespace Coursework_Master_Degree.Player
{
    public class Movement : MonoBehaviour
    {
        public Rigidbody PlayerRigidbody;
        public Transform EyesTransform;

        public InputActionAsset GlobalInputAction;

        public float MoveSpeed;
        public float SpedUpMoveSpeed;
        public float SlowDownMoveSpeed;
        public float CrouchMoveSpeed;

        public float LookSpeed;
        public float UpLimitLookAngle;
        public float DownLimitLookAngle;

        [Min(0.3f)]
        public float CrouchScale;
        [Min(0.1f)]
        public float CrouchDuration;

        private float _yaw;
        private float _pitch;

        private InputAction _moveInputAction;
        private InputAction _speedUpInputAction;
        private InputAction _slowDownInputAction;
        private InputAction _lookInputAction;
        private InputAction _crouchInputAction;

        private Vector2 _moveValue;
        private float _speedUpValue;
        private float _slowDownValue;
        private Vector2 _lookValue;
        private float _crouchValue;

        private Vector3 _crouchScale;
        private Vector3 _regularScale;
        private bool _isToCrouch;
        private bool _isCrouchTransition;
        private float _crouchTimePassed;
        private Vector3 _scaleFrom;
        private Vector3 _scaleTo;

        void Awake()
        {
            HideCursor();

            _crouchScale = new Vector3(1f, CrouchScale, 1f);
            _regularScale = Vector3.one;

            if (UpLimitLookAngle >= DownLimitLookAngle)
            {
                Debug.LogError("UpLimitLookAngle must be smaller than DownLimitLookAngle");
            }

            InputActionMap inputActionMap = GlobalInputAction.FindActionMap("player", true);
            _moveInputAction = inputActionMap.FindAction("move", true);
            _speedUpInputAction = inputActionMap.FindAction("speed_up", true);
            _slowDownInputAction = inputActionMap.FindAction("slow_down", true);
            _lookInputAction = inputActionMap.FindAction("look", true);
            _crouchInputAction = inputActionMap.FindAction("crouch", true);
        }

        private void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        void OnEnable()
        {
            _moveInputAction.Enable();
            _speedUpInputAction.Enable();
            _slowDownInputAction.Enable();
            _lookInputAction.Enable();
            _crouchInputAction.Enable();
        }

        void OnDisable()
        {
            _moveInputAction.Disable();
            _speedUpInputAction.Disable();
            _slowDownInputAction.Disable();
            _lookInputAction.Disable();
            _crouchInputAction.Disable();
        }

        void Update()
        {
            _moveValue = _moveInputAction.ReadValue<Vector2>();
            _speedUpValue = _speedUpInputAction.ReadValue<float>();
            _slowDownValue = _slowDownInputAction.ReadValue<float>();
            _lookValue = _lookInputAction.ReadValue<Vector2>();
            _crouchInputAction.started += 
                (context) =>
                {
                    _crouchValue = _crouchInputAction.ReadValue<float>();
                };

            // if (_crouchValue > 0.5f)
            // {
            //     Vector3 crouchToggleScale = new Vector3(0f, 0.5f, 0f);
            //     if (_isCrouch)
            //     {
            //         PlayerRigidbody.transform.localScale += crouchToggleScale;
            //     }
            //     else
            //     {
            //         PlayerRigidbody.transform.localScale -= crouchToggleScale;
            //     }
            //     _isCrouch = !_isCrouch;
            //     _crouchValue = default;
            // }

            CalculateLook();

            EyesTransform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);

            ApplyCrouch();
        }

        private void CalculateLook()
        {
            _yaw += _lookValue.x * LookSpeed * Time.deltaTime;
            _pitch -= _lookValue.y * LookSpeed * Time.deltaTime;
            _pitch = Mathf.Clamp(_pitch, UpLimitLookAngle, DownLimitLookAngle);
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
                _crouchTimePassed += Time.deltaTime / CrouchDuration;
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
            PlayerRigidbody.MoveRotation(Quaternion.Euler(0f, _yaw, 0f));

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