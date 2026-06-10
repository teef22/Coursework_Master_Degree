using UnityEngine;
using NaughtyAttributes;
using Coursework_Master_Degree.Player;
using Coursework_Master_Degree.UserInterface.Items;

namespace Coursework_Master_Degree.Items.Placement
{
    public class ExistingItemPlacementManager : MonoBehaviour /* : ItemPlacementManager -> which inherits monobehaviour, where only update method is implemented */
    {                                                         /* to calculate item spawn position, and common fields are defined in base class                   */
        // naughty attributes enableif / disableif states
        private bool NotEnabled => false;
        private bool Enabled => true;

        public Transform PlayerEyesTransform;
        public Movement PlayerMovement;
        public MovementPlacementMode PlayerMovementPlacementMode;

        public Material PlacementModeRightMaterial;
        public Material PlacementModeWrongMaterial;

        [EnableIf("NotEnabled")]
        [Tooltip("Set via script; exposed as \"not enabled\" to enhance understanding of the script functionality")]
        public GameObject ExistingItemGameObjectToPlace;

        void Update()
        {
            // ...
        }

        void OnEnable()
        {
            PlayerMovement.enabled = false;

            // player look sync
            PlayerMovementPlacementMode.Yaw = PlayerMovement.Yaw;
            PlayerMovementPlacementMode.Pitch = PlayerMovement.Pitch;

            PlayerMovementPlacementMode.enabled = true;
        }

        void OnDisable()
        {
            PlayerMovementPlacementMode.enabled = false;

            // player look sync
            PlayerMovement.Yaw = PlayerMovementPlacementMode.Yaw;
            PlayerMovement.Pitch = PlayerMovementPlacementMode.Pitch;

            PlayerMovement.enabled = true;

            // reset state
            ExistingItemGameObjectToPlace = null;
        }
    }
}
