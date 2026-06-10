using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using Coursework_Master_Degree.ScriptableObjects.Items;

namespace Coursework_Master_Degree.UserInterface.PrefabData.ScrollablePick
{
    public class ItemDataHolder : MonoBehaviour
    {
        // naughty attributes enableif / disableif states
        private bool NotEnabled => false;
        private bool Enabled => true;

        [EnableIf("NotEnabled")]
        [Tooltip("Set via script")]
        public ItemSO ItemSO;

        public Image IconImage;
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI PriceText;

        [EnableIf("NotEnabled")]
        [Tooltip("Set via script")]
        public TextMeshProUGUI DescriptionHeaderText;
        [EnableIf("NotEnabled")]
        [Tooltip("Set via script")]
        public TextMeshProUGUI DescriptionBodyText;
    }
}
