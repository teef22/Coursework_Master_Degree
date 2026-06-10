using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using Coursework_Master_Degree.ScriptableObjects.Items;

namespace Coursework_Master_Degree.Items
{
    public class DataHolder : MonoBehaviour
    {
        // naughty attributes enableif / disableif states
        private bool NotEnabled => false;
        private bool Enabled => true;

        public ItemSO ItemData;
        public List<MeshRenderer> MeshRenderersList;
        public List<GameObject> CornersList;

        [EnableIf("NotEnabled")]
        [Tooltip("Set via script; scene game object reference")]
        public GameObject RightGameObject;
        [EnableIf("NotEnabled")]
        [Tooltip("Set via script; scene game object reference")]
        public GameObject WrongGameObject;

        // TODO: implement reset state function:
        // expose list to store gameobjects
        // remember all transforms
        // on reset call -> restore transforms from memory
        //
        // pbly store rigid bodies list and remember state of theirs transform
    }
}