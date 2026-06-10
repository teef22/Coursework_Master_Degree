using UnityEngine;
using UnityEngine.InputSystem;

namespace Coursework_Master_Degree.Items.Placement
{
    public class ExistingItemPlacementInputManager : MonoBehaviour
    {
        public InputActionAsset GlobalInputAction;

        public NewItemPlacementManager NewItemPlacementManager;

        [Tooltip("Rotation of item in degrees per input value")]
        public float ItemRotationPerInput;

        private InputAction _placeItemInputAction;
        private InputAction _rotateItemInputAction;

        private float _placeItemValue;
        private Vector2 _rotateItemValue;

        void Awake()
        {
            InputActionMap inputActionMap = GlobalInputAction.FindActionMap("item_placement_movement", true);
            _placeItemInputAction = inputActionMap.FindAction("place_item", true);
            _rotateItemInputAction = inputActionMap.FindAction("rotate_item", true);
        }

        void OnEnable()
        {
            _placeItemInputAction.Enable();
            _rotateItemInputAction.Enable();
        }

        void OnDisable()
        {
            _placeItemInputAction.Disable();
            _rotateItemInputAction.Disable();
        }

        void Update()
        {
            _placeItemValue = _placeItemInputAction.ReadValue<float>();
            _rotateItemValue = _rotateItemInputAction.ReadValue<Vector2>();

            if (_placeItemValue > 0.5f)
            {
                NewItemPlacementManager.PlaceItem();
                enabled = false;
            }

            if (_rotateItemValue.y > 0.5f)
            {
                NewItemPlacementManager.RotateItemRotation(ItemRotationPerInput);
            }
            else if (_rotateItemValue.y < -0.5f)
            {
                NewItemPlacementManager.RotateItemRotation(-ItemRotationPerInput);
            }
        }
    }
}
