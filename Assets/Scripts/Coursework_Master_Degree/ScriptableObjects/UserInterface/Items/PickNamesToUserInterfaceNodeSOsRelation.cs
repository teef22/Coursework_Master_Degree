using System;
using NaughtyAttributes;

namespace Coursework_Master_Degree.ScriptableObjects.UserInterface.Items
{
    [Serializable]
    public class PickNamesToUserInterfaceNodeSOsRelation
    {
        public string PickName;
        [Expandable]
        public UserInterfaceNodeSO UserInterfaceNodeSO;
    }

}
