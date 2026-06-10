using UnityEngine;
using Coursework_Master_Degree.ScriptableObjects.Items.Types;

namespace Coursework_Master_Degree.ScriptableObjects.Items
{
    [CreateAssetMenu(fileName = "item_SO", menuName = "Game Items/Item SO")]
    public class ItemSO : ScriptableObject
    {
        public string Id;

        public string Name;
        [TextArea]
        public string Description;

        public float Price;

        public GameObject Prefab;

        public Texture2D Icon;

        public ItemType[] ItemTypes;
    }
}