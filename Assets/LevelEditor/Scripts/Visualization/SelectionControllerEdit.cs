#if UNITY_EDITOR
using BaseSystem;
using Framework.Helper;
using LevelEditor.Scripts.LeftSide;
using UnityEngine;

namespace LevelEditor.Scripts.Visualization
{
    public class SelectionControllerEdit : ASelectionController
    {
        [Header("Child References")]
        [SerializeField] private BlockTypeSelection blockTypeSelection;
        
        protected override void HandleMouseDown()
        {
            Vector3 touchPos = CameraHelper.GetMouseWorldPosTitledCamera2D();
                
            int numHits = Physics2D.OverlapPointNonAlloc(touchPos, colliders);
            if (numHits > 0)
            {
                ISelectable selectable = colliders[0].GetComponent<ISelectable>();
                if (selectable != null)
                {
                    if (blockTypeSelection.CanRemove)
                    {
                        if (selectable is BlockControllerEdit block)
                        {
                            blockTypeSelection.Blocks.Remove(block);
                            Destroy(block.gameObject);
                        }
                    }
                    else
                    {
                        selectedObject = selectable as BlockControllerEdit;
                        selectable.OnSelect(touchPos);
                    }
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
#endif