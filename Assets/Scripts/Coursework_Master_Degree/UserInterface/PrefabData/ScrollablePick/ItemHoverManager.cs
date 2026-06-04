using UnityEngine;
using UnityEngine.EventSystems;
using Action = System.Action;

namespace Coursework_Master_Degree.UserInterface.Items.ScrollablePick
{
    public class ItemHoverManager : MonoBehaviour, IPointerEnterHandler
    {
        public Action OnPointerEnterAction;

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnterAction?.Invoke();
        }

        public void ResetSelection()
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
