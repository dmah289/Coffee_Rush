using UnityEngine;

namespace BaseSystem
{
    public interface ISelectable
    {
        void OnSelect(Vector3 mousePos);
        void OnDrag(Vector3 currTouchPos);
        void OnDeselect();
    }
}