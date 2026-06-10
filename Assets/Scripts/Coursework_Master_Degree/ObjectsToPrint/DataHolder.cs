using UnityEngine;
using System.Collections.Generic;
using Coursework_Master_Degree.ScriptableObjects.ObjectsToPrint;

namespace Coursework_Master_Degree.ObjectsToPrint
{
    public class DataHolder : MonoBehaviour
    {
        public ObjectToPrintSO ObjectToPrintData;
        public List<MeshRenderer> MeshRenderersList;
        public List<GameObject> CornersList;
    }
}
