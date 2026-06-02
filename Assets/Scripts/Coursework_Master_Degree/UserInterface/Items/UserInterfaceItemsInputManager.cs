using UnityEngine;
using UnityEngine.InputSystem;

namespace Coursework_Master_Degree.UserInterface.Items
{
    public class UserInterfaceItemsInputManager : MonoBehaviour
    {
        public InputActionAsset GlobalInputAction;

        public UserInterfaceItemsNetManager UserInterfaceItemsNetCreator;
        public MonoBehaviour Movement;

        private InputAction _showCoreItemPickMenuInterfaceInputAction;
        private InputAction _hideAnyMenuInterfaceInputAction;

        private float _showCoreItemPickMenuInterfaceValue;
        private float _hideAnyMenuInterfaceValue;

        private bool _isCoreItemPickMenuInterfaceShown;

        void Awake()
        {
            InputActionMap inputActionMap = GlobalInputAction.FindActionMap("user_interface", true);
            _showCoreItemPickMenuInterfaceInputAction = inputActionMap.FindAction("show_core_item_pick_menu_interface", true);
            _hideAnyMenuInterfaceInputAction = inputActionMap.FindAction("hide_any_menu_interface", true);
        }

        void OnEnable()
        {
            _showCoreItemPickMenuInterfaceInputAction.Enable();
            _hideAnyMenuInterfaceInputAction.Enable();
        }

        void OnDisable()
        {
            _showCoreItemPickMenuInterfaceInputAction.Disable();
            _hideAnyMenuInterfaceInputAction.Disable();
        }

        void Update()
        {
            _showCoreItemPickMenuInterfaceValue = _showCoreItemPickMenuInterfaceInputAction.ReadValue<float>();
            _hideAnyMenuInterfaceValue = _hideAnyMenuInterfaceInputAction.ReadValue<float>();

            if (_showCoreItemPickMenuInterfaceValue > 0.5f)
            {
                if (!_isCoreItemPickMenuInterfaceShown)
                {
                    ShowCursor();
                    Movement.enabled = false;

                    UserInterfaceItemsNetCreator.CoreItemPickCanvasGameObjects.SetActive(true);

                    _isCoreItemPickMenuInterfaceShown = true;
                }
            }

            if (_hideAnyMenuInterfaceValue > 0.5f)
            {
                if (_isCoreItemPickMenuInterfaceShown)
                {
                    HideCursor();
                    Movement.enabled = true;

                    UserInterfaceItemsNetCreator.LastActiveItemsPickCanvasGameObject.SetActive(false);

                    _isCoreItemPickMenuInterfaceShown = false;
                }
            }
        }

        private void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void ShowCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
