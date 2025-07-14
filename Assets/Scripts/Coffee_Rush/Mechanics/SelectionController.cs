using BaseSystem;
using DG.Tweening;
using Framework.Helper;
using LevelEditor.Scripts.Visualization;
using UnityEngine;

namespace Coffee_Rush.Mechanics
{
    public class SelectionController : ASelectionController
    {
        protected override void HandleMouseDown()
        {
            Vector3 touchPos = CameraHelper.GetMouseWorldPosTitledCamera2D();
            
            int numHits = Physics2D.OverlapPointNonAlloc(touchPos, colliders);
            if (numHits > 0)
            {
                ISelectable selectable = colliders[0].GetComponent<ISelectable>();
                if (selectable != null)
                {
                    selectedObject = selectable as BlockControllerEdit;
                    selectable.OnSelect(touchPos);
                }
            }
        }

        protected override void HandleMouseDrag()
        {
            Vector3 mousePos = CameraHelper.GetMouseWorldPosTitledCamera2D();
            selectedObject.OnDrag(mousePos);
        }
    }
}