using System;
using UnityEngine;
using NaughtyAttributes;
using Coursework_Master_Degree.Player;
using Coursework_Master_Degree.UserInterface.Items;
using Coursework_Master_Degree.ScriptableObjects.Items;
using System.Linq;
using Coursework_Master_Degree.ScriptableObjects.Items.Types;

namespace Coursework_Master_Degree.Items.Placement
{
    public class NewItemPlacementManager : MonoBehaviour
    {
        // naughty attributes enableif / disableif states
        private bool NotEnabled => false;
        private bool Enabled => true;

        public Transform PlayerEyesTransform;
        public Movement PlayerMovement;
        public MovementPlacementMode PlayerMovementPlacementMode;

        public Material PlacementModeRightMaterial;
        public Material PlacementModeWrongMaterial;

        public UserInterfaceItemsInputManager UserInterfaceItemsInputManager;

        public LayerMask SurfaceLayerMask;
        public LayerMask ShelfLayerMask;
        private LayerMask _layerMask;

        public float MaxPlacementDistance;
        [Tooltip("")]
        public float CornerDeltaDistance;
        [Tooltip("Small offset so the ray does not start exactly on the surface")]
        public float SurfaceNormalOffset;

        [EnableIf("NotEnabled")]
        [Tooltip("Set via script; exposed as \"not enabled\" to enhance understanding of the script functionality")]
        public ItemSO NewItemSODataToPlace;

        private GameObject _itemGameObject;
        private DataHolder _dataHolder;

        private bool _isRightPlacementSpot;
        private bool _lastIsRightPlacementSpotValue;
        private RaycastHit _surfaceHit;
        private Vector3 _placementPosition;

        private bool _isPlaceItem;

        private bool _isRotationChanged;
        private float _itemRotation;

        void Update()
        {
            if (_isPlaceItem && _isRightPlacementSpot)
            {
                enabled = false;
                return;
            }

            else if (_isPlaceItem && !_isRightPlacementSpot)
            {
                _isPlaceItem = false;
            }

            bool isSurfaceHit =
                Physics.Raycast(
                    PlayerEyesTransform.position,
                    PlayerEyesTransform.forward,
                    out _surfaceHit,
                    MaxPlacementDistance,
                    _layerMask,
                    QueryTriggerInteraction.Ignore);

            if (!isSurfaceHit)
            {
                _isRightPlacementSpot = false;
                _placementPosition =
                    PlayerEyesTransform.position +
                    PlayerEyesTransform.forward *
                    MaxPlacementDistance;
            }
            else
            {
                _placementPosition = _surfaceHit.point;

                _isRightPlacementSpot = AreAllCornersSupported(_surfaceHit);
            }

            if (_isRotationChanged)
            {
                _itemGameObject.transform.rotation = Quaternion.Euler(0f, _itemRotation, 0f);
                _dataHolder.RightGameObject.transform.rotation = Quaternion.Euler(0f, _itemRotation, 0f);
                _dataHolder.WrongGameObject.transform.rotation = Quaternion.Euler(0f, _itemRotation, 0f);
                
                _isRotationChanged = false;
            }

            _itemGameObject.transform.position = _placementPosition;
            _dataHolder.RightGameObject.transform.position = _placementPosition;
            _dataHolder.WrongGameObject.transform.position = _placementPosition;

            // avoid SetActive calls on each update, if there is no state change
            if (_lastIsRightPlacementSpotValue != _isRightPlacementSpot)
            {
                if (_isRightPlacementSpot)
                {
                    _dataHolder.RightGameObject.SetActive(true);
                    _dataHolder.WrongGameObject.SetActive(false);
                }
                else
                {
                    _dataHolder.RightGameObject.SetActive(false);
                    _dataHolder.WrongGameObject.SetActive(true);
                }
            }

            _lastIsRightPlacementSpotValue = _isRightPlacementSpot;
        }

        private bool AreAllCornersSupported(RaycastHit surfaceHit)
        {
            Vector3 probeDirection = Vector3.down;

            for (int cornerIndex = 0; cornerIndex < _dataHolder.CornersList.Count; cornerIndex++)
            {
                Vector3 probeOrigin =
                    _dataHolder.CornersList[cornerIndex].transform.position +
                    surfaceHit.normal * SurfaceNormalOffset;

                if (!Physics.Raycast(
                        probeOrigin,
                        probeDirection,
                        CornerDeltaDistance,
                        _layerMask,
                        QueryTriggerInteraction.Ignore))
                {
                    return false;
                }
            }

            return true;
        }

        void OnEnable()
        {
            UserInterfaceItemsInputManager.enabled = false;
            PlayerMovement.enabled = false;

            // player look sync
            PlayerMovementPlacementMode.Yaw = PlayerMovement.Yaw;
            PlayerMovementPlacementMode.Pitch = PlayerMovement.Pitch;

            PlayerMovementPlacementMode.enabled = true;

            CreateAndAssignItem();

            if (_dataHolder.ItemData.ItemTypes.Contains(ItemType.Surface))
            {
                _layerMask = SurfaceLayerMask;
            }
            else
            {
                if (_dataHolder.ItemData.ItemTypes.Contains(ItemType.Enclosure))
                {
                    // for this case I better store colliders in DataHolder class too
                }
                _layerMask = SurfaceLayerMask | ShelfLayerMask;
            }
        }

        private void CreateAndAssignItem()
        {
            _itemGameObject = Instantiate(NewItemSODataToPlace.Prefab);
            _itemGameObject.SetActive(false);

            _dataHolder = _itemGameObject.GetComponent<DataHolder>();

            _dataHolder.RightGameObject = InstantiateForPlacementMode(PlacementStatus.Right);
            _dataHolder.WrongGameObject = InstantiateForPlacementMode(PlacementStatus.Wrong);
        }

        private enum PlacementStatus { Right, Wrong }

        private GameObject InstantiateForPlacementMode(PlacementStatus placementStatus)
        {
            GameObject itemGameObjectPlacementStatus = Instantiate(NewItemSODataToPlace.Prefab);
            itemGameObjectPlacementStatus.SetActive(false);

            DataHolder dataHolder = itemGameObjectPlacementStatus.GetComponent<DataHolder>();

            for (int indexList = 0; indexList < dataHolder.MeshRenderersList.Count; indexList++)
            {
                MeshRenderer meshRenderer = dataHolder.MeshRenderersList[indexList];

                Material[] materials = meshRenderer.materials;

                for (int indexMaterial = 0; indexMaterial < materials.Length; indexMaterial++)
                {
                    materials[indexMaterial] = GetStatusMaterial(placementStatus);
                }

                meshRenderer.materials = materials;
            }

            return itemGameObjectPlacementStatus;
        }

        private Material GetStatusMaterial(PlacementStatus placementStatus)
        {
            switch (placementStatus)
            {
                case PlacementStatus.Right:
                    return PlacementModeRightMaterial;
                case PlacementStatus.Wrong:
                    return PlacementModeWrongMaterial;
                default:
                    Debug.Log("Unexpected placement status");
                    throw new Exception("Unexpected placement status");
            }
        }

        void OnDisable()
        {
            UserInterfaceItemsInputManager.enabled = true;
            PlayerMovementPlacementMode.enabled = false;

            // player look sync
            PlayerMovement.Yaw = PlayerMovementPlacementMode.Yaw;
            PlayerMovement.Pitch = PlayerMovementPlacementMode.Pitch;

            PlayerMovement.enabled = true;

            _itemGameObject.SetActive(true);
            _dataHolder.RightGameObject.SetActive(false);
            _dataHolder.WrongGameObject.SetActive(false);

            ResetItemState();
        }

        private void ResetItemState()
        {
            NewItemSODataToPlace = default;
            _itemGameObject = default;
            _dataHolder = default;

            _layerMask = default;

            _isPlaceItem = default;
            _itemRotation = default;

            _isRightPlacementSpot = default;
            _lastIsRightPlacementSpotValue = default;
            _surfaceHit = default;
            _placementPosition = default;

            _isPlaceItem = default;

            _isRotationChanged = default;
            _itemRotation = default;
        }

        public void PlaceItem()
        {
            _isPlaceItem = true;
        }

        public void RotateItemRotation(float value)
        {
            _itemRotation += value;
            _itemRotation = Mathf.Repeat(_itemRotation, 360f);
            
            _isRotationChanged = true;
        }
    }
}
