using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using Coursework_Master_Degree.ScriptableObjects.UserInterface.Items;
using Coursework_Master_Degree.ScriptableObjects.UserInterface.Items.Types;
using Coursework_Master_Degree.UserInterface.PrefabData.RegularPick;
using Coursework_Master_Degree.UserInterface.PrefabData.ScrollablePick;
using Coursework_Master_Degree.UserInterface.Items.ScrollablePick;

namespace Coursework_Master_Degree.UserInterface.Items
{
    public class UserInterfaceItemsNetManager : MonoBehaviour
    {
        [Expandable]
        public UserInterfaceNodeSO CoreUserInterfaceNodeSO;

        [Header("Regular Pick")]
        [Space(10)]
        public GameObject ItemRegularPickCanvas;
        public GameObject Option;

        [Header("Scrollable Pick")]
        [Space(10)]
        public GameObject ItemScrollablePickCanvas;
        public GameObject Row;
        public GameObject Item;

        private GameObject _coreItemPickCanvasGameObjects;
        public GameObject CoreItemPickCanvasGameObjects {
            get {
                _lastActiveItemPickCanvasGameObjectUserInterfaceNodeType = UserInterfaceNodeType.RegularPick;
                _lastActiveItemPickCanvasGameObject = _coreItemPickCanvasGameObjects;

                return _coreItemPickCanvasGameObjects;
            }
        }

        private UserInterfaceNodeType _lastActiveItemPickCanvasGameObjectUserInterfaceNodeType;
        private GameObject _lastActiveItemPickCanvasGameObject;
        public GameObject LastActiveItemsPickCanvasGameObject {
            get {
                ResetLastActiveItemPickInterfaceScroll();

                return _lastActiveItemPickCanvasGameObject;
            }
        }

        private void ResetLastActiveItemPickInterfaceScroll()
        {
            switch (_lastActiveItemPickCanvasGameObjectUserInterfaceNodeType)
            {
                case UserInterfaceNodeType.RegularPick:
                    ResetItemRegularPickInterfaceScroll();
                    break;
                case UserInterfaceNodeType.ScrollablePick:
                    ResetItemScrollablePickInterfaceScroll();
                    break;
                default:
                    Debug.LogError("Unexpected user interface node type");
                    throw new System.Exception("Unexpected user interface node type");
            }
        }

        private void ResetItemRegularPickInterfaceScroll()
        {
            // reset vertical scroll of item regular picker
            ItemRegularPickCanvasDataHolder itemRegularPickCanvasDataHolder = _lastActiveItemPickCanvasGameObject.GetComponent<ItemRegularPickCanvasDataHolder>();
            // reset position
            itemRegularPickCanvasDataHolder.VerticalScrollRect.verticalNormalizedPosition = 1.0f;
        }

        private void ResetItemScrollablePickInterfaceScroll()
        {
            // reset vertical scroll of item scrollable picker
            ItemScrollablePickCanvasDataHolder itemScrollablePickCanvasDataHolder = _lastActiveItemPickCanvasGameObject.GetComponent<ItemScrollablePickCanvasDataHolder>();
            // reset position
            itemScrollablePickCanvasDataHolder.VerticalScrollRect.verticalNormalizedPosition = 1.0f;

            // reset horizontal scroll of each row
            int numberOfRows = itemScrollablePickCanvasDataHolder.ContentGameObject.transform.childCount;
            for (int rowIndex = 0; rowIndex < numberOfRows; rowIndex++)
            {
                GameObject rowGameObject = itemScrollablePickCanvasDataHolder.ContentGameObject.transform.GetChild(rowIndex).gameObject;
                RowDataHolder rowDataHolder = rowGameObject.GetComponent<RowDataHolder>();
                // reset position
                rowDataHolder.CustomRowScrollRect.horizontalNormalizedPosition = 0.0f;
            }
        }

        void Awake()
        {
            _coreItemPickCanvasGameObjects = CreateItemPickCanvasGameObject(CoreUserInterfaceNodeSO);
            _coreItemPickCanvasGameObjects.SetActive(false);
        }

        private GameObject CreateItemPickCanvasGameObject(UserInterfaceNodeSO userInterfaceNodeSO)
        {
            switch (userInterfaceNodeSO.UserInterfaceNodeType)
            {
                case UserInterfaceNodeType.RegularPick:
                    return CreateItemRegularPickCanvasGameObject(userInterfaceNodeSO);
                case UserInterfaceNodeType.ScrollablePick:
                    return CreateItemScrollablePickCanvasGameObject(userInterfaceNodeSO);
                default:
                    Debug.LogError("Unexpected user interface node type");
                    throw new System.Exception("Unexpected user interface node type");
            }
        }

        private GameObject CreateItemRegularPickCanvasGameObject(UserInterfaceNodeSO userInterfaceNodeSO)
        {
            GameObject itemRegularPickCanvasGameObject;

            // create regular item picker
            itemRegularPickCanvasGameObject = Instantiate(ItemRegularPickCanvas, gameObject.transform);
            itemRegularPickCanvasGameObject.SetActive(false);
            // retrieve data holder
            ItemRegularPickCanvasDataHolder itemRegularPickCanvasDataHolder = itemRegularPickCanvasGameObject.GetComponent<ItemRegularPickCanvasDataHolder>();
            // get content
            GameObject contentGameObject = itemRegularPickCanvasDataHolder.ContentGameObject;

            // loop to create options
            int numberOfOptions = userInterfaceNodeSO.PickNamesToUserInterfaceNodeSOs.Count;
            for (int optionIndex = 0; optionIndex < numberOfOptions; optionIndex++)
            {
                // create option
                GameObject option = Instantiate(Option, contentGameObject.transform);
                // retrieve data holder
                OptionDataHolder optionDataHolder = option.GetComponent<OptionDataHolder>();

                // get name
                string optionName = userInterfaceNodeSO.PickNamesToUserInterfaceNodeSOs[optionIndex].PickName;
                // set name
                optionDataHolder.OptionText.text = optionName;

                // get child level user interface node
                UserInterfaceNodeSO childLevelUserInterfaceNodeSO = userInterfaceNodeSO.PickNamesToUserInterfaceNodeSOs[optionIndex].UserInterfaceNodeSO;
                // get child level user interface node type
                UserInterfaceNodeType childLevelUserInterfaceNodeType = childLevelUserInterfaceNodeSO.UserInterfaceNodeType;
                // get item picker for button onclick event
                GameObject childLevelItemPickCanvasGameObject = CreateItemPickCanvasGameObject(childLevelUserInterfaceNodeSO);

                // get button component
                Button optionButton = option.GetComponent<Button>();
                // set button onclick event
                optionButton.onClick.AddListener(
                    () =>
                    {
                        ResetLastActiveItemPickInterfaceScroll();

                        itemRegularPickCanvasGameObject.SetActive(false);
                        childLevelItemPickCanvasGameObject.SetActive(true);

                        _lastActiveItemPickCanvasGameObjectUserInterfaceNodeType = childLevelUserInterfaceNodeType;
                        _lastActiveItemPickCanvasGameObject = childLevelItemPickCanvasGameObject;
                    });
            }

            return itemRegularPickCanvasGameObject;
        }

        private GameObject CreateItemScrollablePickCanvasGameObject(UserInterfaceNodeSO userInterfaceNodeSO)
        {
            GameObject itemScrollablePickCanvasGameObject;

            // create scrollable item picker
            itemScrollablePickCanvasGameObject = Instantiate(ItemScrollablePickCanvas, gameObject.transform);
            itemScrollablePickCanvasGameObject.SetActive(false);
            // retrieve data holder
            ItemScrollablePickCanvasDataHolder itemScrollablePickCanvasDataHolder = itemScrollablePickCanvasGameObject.GetComponent<ItemScrollablePickCanvasDataHolder>();
            // get content
            GameObject pickerContentGameObject = itemScrollablePickCanvasDataHolder.ContentGameObject;

            // loop to create rows
            int numberOfRows = userInterfaceNodeSO.ItemsListSOs.Count;
            for (int rowIndex = 0; rowIndex < numberOfRows; rowIndex++)
            {
                // create row
                GameObject rowGameObject = Instantiate(Row, pickerContentGameObject.transform);
                // retrieve data holder
                RowDataHolder rowDataHolder = rowGameObject.GetComponent<RowDataHolder>();

                // get horizontal scroll rect
                CustomRowScrollRect customRowScrollRect = rowDataHolder.CustomRowScrollRect;
                // set parent vertical scroll rect
                customRowScrollRect.ParentScrollRect = itemScrollablePickCanvasDataHolder.VerticalScrollRect;

                // set header text
                rowDataHolder.HeaderText.text = userInterfaceNodeSO.ItemsListSOs[rowIndex].ItemsListName;

                // get content
                GameObject rowContentGameObject = rowDataHolder.ContentGameObject;

                // loop to create items
                int numberOfItems = userInterfaceNodeSO.ItemsListSOs[rowIndex].ItemsList.Length;
                for (int itemIndex = 0; itemIndex < numberOfItems; itemIndex++)
                {
                    // create item
                    GameObject itemGameObject = Instantiate(Item, rowContentGameObject.transform);
                    // retrieve data holder
                    ItemsDataHolder itemsDataHolder = itemGameObject.GetComponent<ItemsDataHolder>();

                    // get item image and create sprite out of it
                    Texture2D itemImage = userInterfaceNodeSO.ItemsListSOs[rowIndex].ItemsList[itemIndex].Icon;
                    Sprite itemImageSprite = Sprite.Create(
                        itemImage,
                        new Rect(0, 0, itemImage.width, itemImage.height),
                        new Vector2(0.5f, 0.5f)
                    );
                    // set item image
                    itemsDataHolder.IconImage.sprite = itemImageSprite;
                    // get item name text
                    string itemName = userInterfaceNodeSO.ItemsListSOs[rowIndex].ItemsList[itemIndex].Name;
                    // set item name text
                    itemsDataHolder.NameText.text = itemName;
                    // get item price text
                    float itemPrice = userInterfaceNodeSO.ItemsListSOs[rowIndex].ItemsList[itemIndex].Price;
                    // set item price text
                    itemsDataHolder.PriceText.text = itemPrice + " $";
                }
            }

            return itemScrollablePickCanvasGameObject;
        }
    }
}
