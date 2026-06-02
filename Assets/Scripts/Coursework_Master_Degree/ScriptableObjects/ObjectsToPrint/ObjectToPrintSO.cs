using UnityEngine;
using Coursework_Master_Degree.ScriptableObjects.ObjectsToPrint.Types;

namespace Coursework_Master_Degree.ScriptableObjects.ObjectsToPrint
{
    [CreateAssetMenu(fileName = "object_to_print_SO", menuName = "Game Items/Object To Print SO")]
    public class ObjectToPrintSO : ScriptableObject
    {
        public string Id;

        public string Name;
        [TextArea]
        public string Description;

        public GameObject Prefab;
        public Texture2D Icon;

        public ObjectToPrintType[] ObjectToPrintTypes;
    }
}