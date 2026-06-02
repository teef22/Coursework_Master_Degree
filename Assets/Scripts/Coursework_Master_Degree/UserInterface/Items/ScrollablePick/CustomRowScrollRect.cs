using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Coursework_Master_Degree.UserInterface.Items.ScrollablePick
{
    public class CustomRowScrollRect : ScrollRect
    {
        [SerializeField] public ScrollRect ParentScrollRect;

        private enum DragRoute
        {
            Undecided,
            Self,
            Parent
        }

        private DragRoute _route = DragRoute.Undecided;
        private Vector2 _accumulatedDelta;

        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            base.OnInitializePotentialDrag(eventData);

            if (ParentScrollRect != null)
                ParentScrollRect.OnInitializePotentialDrag(eventData);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            _route = DragRoute.Undecided;
            _accumulatedDelta = Vector2.zero;
            // Do not call base yet. Wait until we know whether this drag is horizontal or vertical.
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (_route == DragRoute.Undecided)
            {
                _accumulatedDelta += eventData.delta;

                // Optional small threshold so tiny movement does not decide too early.
                if (_accumulatedDelta.sqrMagnitude < 9f)
                    return;

                _route = Mathf.Abs(_accumulatedDelta.y) > Mathf.Abs(_accumulatedDelta.x)
                    ? DragRoute.Parent
                    : DragRoute.Self;

                if (_route == DragRoute.Parent)
                    ParentScrollRect?.OnBeginDrag(eventData);
                else
                    base.OnBeginDrag(eventData);
            }

            if (_route == DragRoute.Parent)
                ParentScrollRect?.OnDrag(eventData);
            else
                base.OnDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (_route == DragRoute.Parent)
                ParentScrollRect?.OnEndDrag(eventData);
            else if (_route == DragRoute.Self)
                base.OnEndDrag(eventData);

            _route = DragRoute.Undecided;
            _accumulatedDelta = Vector2.zero;
        }
    }
}
