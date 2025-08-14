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

        [SerializeField] private LayerMask gridLayer;
        [SerializeField] private Camera cam;
        private RaycastHit[] hits = new RaycastHit[1];
        private Vector3 outOfViewPos = new (int.MaxValue, int.MaxValue, int.MaxValue);
        

        protected override void Awake()
        {
            base.Awake();
            
            // TODO : Reset this value when player restart the level
            isFirstBlockMoved = true;
        }
        
        private void Update()
        {
#if UNITY_EDITOR
            HandleOnEditor();
#elif UNITY_ANDROID || UNITY_IOS
            HandleOnMobile();
#endif
        }

        private void HandleOnMobile()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        HandleMouseDown();
                        break;
                    
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        if(selectedObject != null)
                            HandleMouseDrag();
                        break;
                    
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        if(selectedObject != null)
                            HandleMouseUp();
                        break;
                }
            }
        }

        private void HandleOnEditor()
        {
            if (Input.GetMouseButtonDown(0))
                HandleMouseDown();
            else if (Input.GetMouseButton(0) && selectedObject != null)
                HandleMouseDrag();
            else if (Input.GetMouseButtonUp(0) && selectedObject != null)
                HandleMouseUp();
        }

        private void HandleMouseDown()
        {
            Vector3 touchPos = GetWorldTouchPosOnGrid();
            
            if (touchPos.Equals(outOfViewPos)) return;
            
            int numHits = Physics2D.OverlapPointNonAlloc(touchPos, colliders);
            if (numHits > 0)
            {
                ISelectable selectable = colliders[0].GetComponentInParent<ISelectable>();
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
            Vector3 mousePos = GetWorldTouchPosOnGrid();

            if (mousePos.Equals(outOfViewPos)) return;
            
            selectedObject?.OnDrag(mousePos);
        }
        
        public void HandleMouseUp()
        {
            selectedObject?.OnDeselect();
            selectedObject = null;
        }

        public void DeselectCurrentObject(ISelectable selectable)
        {
            if (selectedObject == selectable)
                HandleMouseUp();
        }

        public void EnterGameplay()
        {
            isFirstBlockMoved = true;
            gameObject.SetActive(true);
        }

        private Vector3 GetWorldTouchPosOnGrid()
        {
            Vector3 screenPos = Input.touchCount > 0
                ? Input.GetTouch(0).position
                : Input.mousePosition;

            Ray ray = cam.ScreenPointToRay(screenPos);
            // Debug.DrawRay(ray.origin, ray.direction * 500f, Color.red, 3f);

            int hitCount = Physics.RaycastNonAlloc(ray, hits, 500f, gridLayer);
    
            if (hitCount > 0)
                return hits[0].point;

            return outOfViewPos;
        }
    }
}