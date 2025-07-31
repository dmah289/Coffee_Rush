using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;

namespace Coffee_Rush.UI.BaseSystem
{
    public abstract class ALoadingPage : MonoBehaviour
    {
        private static readonly int FillAmount = Shader.PropertyToID("_FillAmount");
        

        [Header("Components")]
        [SerializeField] private RectTransform fillRectTransform;
        

        protected abstract UniTaskVoid OnFullFillAmount();

        protected async UniTaskVoid StartLoading()
        {
            float timer = 0f;
            float curWidth = 0;
            fillRectTransform.sizeDelta = new Vector2(curWidth, 58);

            while (timer < 3.5f)
            {
                timer += Time.deltaTime;

                curWidth = (timer / 3.5f) * 820;
                fillRectTransform.sizeDelta = new Vector2(curWidth, 58);
                
                await UniTask.Yield();
            }
            
            fillRectTransform.sizeDelta = new Vector2(820, 58);
            OnFullFillAmount();
        }
    }
}