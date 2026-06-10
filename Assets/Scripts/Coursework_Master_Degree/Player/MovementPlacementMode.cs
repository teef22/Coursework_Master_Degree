using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Coursework_Master_Degree.Player
{
    public class MovementPlacementMode : MonoBehaviour
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
        [EnableIf("NotEnabled")]
        [Tooltip("Based on regular movement move speed")]
        [Min(0.01f)] public float MoveSpeed;

        [Header("Look")]
        [EnableIf("NotEnabled")]
        [Tooltip("Based on regular movement look speed")]
        [Min(0.01f)] public float LookSpeed;
        [EnableIf("NotEnabled")]
        [Tooltip("Based on regular movement up limit look angle")]
        [Min(0.01f)] public float UpLimitLookAngle;
        [EnableIf("NotEnabled")]
        [Tooltip("Based on regular movement down limit look angle")]
        [Min(0.01f)] public float DownLimitLookAngle;

        [Header("Crouch")]
        [EnableIf("NotEnabled")]
        [Tooltip("Based on regular movement crouch scale")]
        [Min(0.3f)] public float CrouchScale;
        [EnableIf("NotEnabled")]
        [Tooltip("Based on regular movement crouch transition duration")]
        [Min(0.1f)] public float CrouchTransitionDuration;

        [EnableIf("NotEnabled")]
        [Tooltip("Exposed to show avaliability, disabled to forbid value set in editor")]
        public float Yaw;
        [EnableIf("NotEnabled")]
        [Tooltip("Exposed to show avaliability, disabled to forbid value set in editor")]
        public float Pitch;

        private InputAction _moveInputAction;
        private InputAction _lookInputAction;
        private InputAction _crouchInputAction;
        private InputAction _placeItemInputAction;

        private Vector2 _moveValue;
        private Vector2 _lookValue;
        private float _crouchValue;
        private float _placeItemValue;

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
            TryRetrieveRegularMovementFieldValues();
        }

        private void TryRetrieveRegularMovementFieldValues()
        {
            if(TryGetComponent(out Movement movement))
            {
                MoveSpeed = movement.SlowDownMoveSpeed;
                LookSpeed = movement.LookSpeed;
                UpLimitLookAngle = movement.UpLimitLookAngle;
                DownLimitLookAngle = movement.DownLimitLookAngle;
                CrouchScale = movement.CrouchScale;
                CrouchTransitionDuration = movement.CrouchTransitionDuration;
            }
            else
            {
                Debug.Log("There is no \"Movement\" component on this Player object to retreive \"SlowDownMoveSpeed\" value from.");
                throw new Exception("There is no \"Movement\" component on this Player object to retreive \"SlowDownMoveSpeed\" value from.");
            }
        }

        void Awake()
        {
            HideCursor();

            TryRetrieveRegularMovementFieldValues();

            _crouchScale = new Vector3(1f, CrouchScale, 1f);
            _regularScale = Vector3.one;

            if (UpLimitLookAngle >= DownLimitLookAngle)
            {
                Debug.LogError("UpLimitLookAngle must be smaller than DownLimitLookAngle");
            }

            InputActionMap inputActionMap = GlobalInputAction.FindActionMap("item_placement_movement", true);
            _moveInputAction = inputActionMap.FindAction("move", true);
            _lookInputAction = inputActionMap.FindAction("look", true);
            _crouchInputAction = inputActionMap.FindAction("crouch", true);
            _placeItemInputAction = inputActionMap.FindAction("place_item", true);
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
            _lookInputAction.Enable();
            _crouchInputAction.Enable();
            _placeItemInputAction.Enable();
        }

        void OnDisable()
        {
            _moveInputAction.Disable();
            _lookInputAction.Disable();
            _crouchInputAction.Disable();
            _placeItemInputAction.Disable();
        }

        void Update()
        {
            _moveValue = _moveInputAction.ReadValue<Vector2>();
            _lookValue = _lookInputAction.ReadValue<Vector2>();
            _crouchInputAction.started += 
                (context) =>
                {
                    _crouchValue = _crouchInputAction.ReadValue<float>();
                };
            // _placeItemValue = _placeItemInputAction.ReadValue<float>();

            CalculateLook();

            EyesTransform.localRotation = Quaternion.Euler(Pitch, 0f, 0f);

            ApplyCrouch();
        }

        private void CalculateLook()
        {
            Yaw += _lookValue.x * LookSpeed * Time.deltaTime;
            Pitch -= _lookValue.y * LookSpeed * Time.deltaTime;
            Pitch = Mathf.Clamp(Pitch, UpLimitLookAngle, DownLimitLookAngle);
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

            Vector3 targetPosition = PlayerRigidbody.position + moveDirection * MoveSpeed * Time.fixedDeltaTime;

            PlayerRigidbody.MovePosition(targetPosition);
        }
    }
}
