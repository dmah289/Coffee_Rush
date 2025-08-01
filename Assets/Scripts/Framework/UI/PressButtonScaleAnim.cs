using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.UI
{
    public class PressButtonScaleAnim : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private RectTransform selfRectTransform;
        
        
        [SerializeField] private UnityEvent OnAnimDone;
        
        
        private Vector3 curScale;
        private Vector3 targetScale = new (0.9f, 0.9f, 0.9f);


        public void OnButtonClicked() => OnButtonClickedAsync().Forget();
        
        
        private async UniTaskVoid OnButtonClickedAsync()
        {
            float timer = 0;
            curScale = Vector3.one;

            while (timer < 0.1f)
            {
                timer += Time.deltaTime;
                curScale = Vector3.Lerp(curScale, targetScale, timer / 0.1f);
                selfRectTransform.localScale = curScale;
                await UniTask.Yield();
            }

            selfRectTransform.localScale = Vector3.one;

            await UniTask.Delay(155);
            
            OnAnimDone?.Invoke();
        }
    }
}