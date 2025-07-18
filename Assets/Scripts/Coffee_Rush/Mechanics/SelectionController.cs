using DG.Tweening;
using Framework.Helper;
using UnityEngine;

namespace BaseSystem
{
    public class SelectionController : MonoBehaviour
    {
        [Header("Selection Settings")]
        private Camera cam;
        private Collider2D[] colliders = new Collider2D[1];
        private ISelectable selectedObject;

        private void Awake()
        {
            cam = Camera.main;
        }
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseDown();
            }
            else if (Input.GetMouseButton(0) && selectedObject != null)
            {
                HandleMouseDrag();
            }
            else if (Input.GetMouseButtonUp(0) && selectedObject != null)
            {
                HandleMouseUp();
            }
        }

        private void HandleMouseUp()
        {
            selectedObject.OnDeselect();
            selectedObject = null;
        }

        private void HandleMouseDown()
        {
            Vector3 touchPos = CameraHelper.GetMouseWorldPosTitledCamera2D();
            
            int numHits = Physics2D.OverlapPointNonAlloc(touchPos, colliders);
            if (numHits > 0)
            {
                ISelectable selectable = colliders[0].GetComponent<ISelectable>();
                if (selectable != null)
                {
                    selectedObject = selectable as ABlockController;
                    selectable.OnSelect(touchPos);
                }
            }
        }

        private void HandleMouseDrag()
        {
            Vector3 mousePos = CameraHelper.GetMouseWorldPosTitledCamera2D();
            selectedObject.OnDrag(mousePos);
        }
    }
}