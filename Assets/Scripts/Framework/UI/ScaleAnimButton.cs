using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Framework.UI
{
    public class ScaleAnimButton : MonoBehaviour, IPointerClickHandler
    {
        private RectTransform selfRectTransform;
        [SerializeField] private Vector3 targetScale = new (1.1f, 1.1f, 1.1f);
        
        [SerializeField] private UnityEvent OnScaleAnimDone;

        private void Awake()
        {
            selfRectTransform = GetComponent<RectTransform>();
        }

        private async UniTaskVoid OnButtonClickedAsync()
        {
            float timer = 0;
            Vector3 curScale = Vector3.one;

            while (timer < 0.1f)
            {
                timer += Time.deltaTime;
                curScale = Vector3.Lerp(curScale, targetScale, timer / 0.1f);
                selfRectTransform.localScale = curScale;
                await UniTask.Yield();
            }

            selfRectTransform.localScale = Vector3.one;

            await UniTask.Delay(155);
            
            OnScaleAnimDone?.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnButtonClickedAsync().Forget();
        }
    }
}