using DG.Tweening;
using UnityEngine;

namespace BaseSystem
{
    public abstract class ASelectionController : MonoBehaviour
    {
        [Header("Selection Settings")]
        protected Camera cam;
        protected private Collider2D[] colliders = new Collider2D[1];
        protected ISelectable selectedObject;
        
        protected abstract void HandleMouseDown();
        protected abstract void HandleMouseDrag();

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
                transform.DOMoveZ(0, 0.2f);
                selectedObject.OnDeselect();
                selectedObject = null;
            }
        }

    }
}