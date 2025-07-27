using BaseSystem.Block;
using Coffee_Rush.Block;
using Coffee_Rush.Level;
using DG.Tweening;
using Framework.DesignPattern;
using Framework.Helper;
using UnityEngine;

namespace BaseSystem
{
    public class SelectionController : MonoSingleton<SelectionController>
    {
        [Header("Selection Settings")]
        private Collider2D[] colliders = new Collider2D[1];
        private ISelectable selectedObject;
        [SerializeField] private bool isFirstBlockMoved;
        
        [Header("References")]
        [SerializeField] private LevelTimer levelTimer;

        protected override void Awake()
        {
            base.Awake();
            
            // TODO : Reset this value when player restart the level
            isFirstBlockMoved = true;
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

        public void HandleMouseUp()
        {
            selectedObject?.OnDeselect();
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
                    if(selectable is ABlockController blockController)
                    {
                        if (blockController is BlockController { CanSelect: false })
                            return;
                        
                        if (isFirstBlockMoved)
                        {
                            levelTimer.StartTimerOnFirstBlockMove();
                            isFirstBlockMoved = false;
                        }
                    }
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