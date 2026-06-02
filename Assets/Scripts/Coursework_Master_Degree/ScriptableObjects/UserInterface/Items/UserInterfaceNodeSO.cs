using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using Coursework_Master_Degree.ScriptableObjects.Items;
using Coursework_Master_Degree.ScriptableObjects.UserInterface.Items.Types;

namespace Coursework_Master_Degree.ScriptableObjects.UserInterface.Items
{
    [CreateAssetMenu(fileName = "User_interface_node_SO", menuName = "User Interface Network/User Interface Node")]
    public class UserInterfaceNodeSO : ScriptableObject
    {
        public UserInterfaceNodeType UserInterfaceNodeType;

        public bool IsRegularPickType() { return UserInterfaceNodeType == UserInterfaceNodeType.RegularPick; }
        [ShowIf("IsRegularPickType")]
        public List<PickNamesToUserInterfaceNodeSOsRelation> PickNamesToUserInterfaceNodeSOs;

        public bool IsScrollablePickType() { return UserInterfaceNodeType == UserInterfaceNodeType.ScrollablePick; }
        [ShowIf("IsScrollablePickType"), Expandable]
        public List<ItemsListSO> ItemsListSOs;
    }
}
