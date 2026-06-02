using UnityEngine;

namespace Coursework_Master_Degree.ScriptableObjects.ObjectsToPrint
{
    [CreateAssetMenu(fileName = "objects_to_print_list_SO", menuName = "Game Items/Objects To Print List SO")]
    public class ObjectsToPrintListSO : ScriptableObject
    {
        public ObjectToPrintSO[] ObjectsToPrintList;
    }
}