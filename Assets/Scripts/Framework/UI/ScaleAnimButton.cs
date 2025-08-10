using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Framework.UI
{
    public class ScaleAnimButton : MonoBehaviour, IPointerClickHandler
    {
        [Header("Self Components")]
        [SerializeField] protected RectTransform selfRectTransform;
        
        [Header("Scale Animation Settings")]
        [SerializeField] protected float targetScale = 1.1f;
        [SerializeField] protected float animDuration = 0.1f;
        [SerializeField] protected float clickIntervalThreshold;
        [SerializeField] protected float actionDelay;
        protected float lastClickTime;
        
        [SerializeField] protected UnityEvent OnScaleAnimDone;

        
        protected bool CanClick
        {
            get
            {
                float timeSinceLastClick = Time.time - lastClickTime;
                if (timeSinceLastClick >= clickIntervalThreshold)
                {
                    lastClickTime = Time.time;
                    return true;
                }
                return false;
            }
        }
        

        protected virtual void Awake()
        {
            selfRectTransform = GetComponent<RectTransform>();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            OnButtonClicked();
        }

        protected virtual void OnButtonClicked()
        {
            selfRectTransform.DOKill();
            selfRectTransform.DOScale(targetScale, animDuration)
                .OnStart(() => selfRectTransform.localScale = Vector3.one)
                .OnComplete(() =>
                {
                    selfRectTransform.localScale = Vector3.one;
                    DOVirtual.DelayedCall(actionDelay, () => OnScaleAnimDone?.Invoke());

                });
        }
    }
}