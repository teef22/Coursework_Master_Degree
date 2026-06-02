using UnityEngine;

namespace Coursework_Master_Degree.ScriptableObjects.Items
{
    [CreateAssetMenu(fileName = "items_list_SO", menuName = "Game Items/Items List SO")]
    public class ItemsListSO : ScriptableObject
    {
        public string ItemsListName;
        public ItemSO[] ItemsList;
    }
}