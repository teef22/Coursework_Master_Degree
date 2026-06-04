using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

namespace Coursework_Master_Degree.UserInterface.PrefabData.ScrollablePick
{
    public class ItemDataHolder : MonoBehaviour
    {
        // naughty attributes enableif / disableif states
        private bool NotEnabled => false;
        private bool Enabled => true;

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
