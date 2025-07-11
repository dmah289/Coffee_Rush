using UnityEngine;

namespace LevelEditor.Scripts.Visualization
{
    public interface IEditorSelectable
    {
        void OnSelect(Vector3 mousePos);
        void OnDrag();
        void OnDeselect();
    }
}