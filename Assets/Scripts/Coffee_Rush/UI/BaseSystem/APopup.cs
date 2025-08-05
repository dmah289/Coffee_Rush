using System;
using Cysharp.Threading.Tasks;
using Framework.Extensions;
using Framework.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee_Rush.UI.MainMenu.Home
{
    public abstract class APopup : MonoBehaviour
    {
        [SerializeField] protected BackgroundClickHandler bgClickHandler;
        [SerializeField] protected RectTransform selfRectTransform;
        [SerializeField] protected Image image;


        protected float ScaleDuration = 0.2f;

        private void Awake()
        {
            bgClickHandler = GetComponentInParent<BackgroundClickHandler>();
            selfRectTransform = GetComponent<RectTransform>();
            image = GetComponent<Image>();
        }

        public void ShowPopup()
        {
            selfRectTransform.localScale = Vector3.one;
            image.SetAlpha(1);
        }

        public void HidePopup() => HidePopupAsync().Forget();
        protected async UniTaskVoid HidePopupAsync()
        {
            image.FadeAlphaToTarget(ScaleDuration / 4).Forget();
            
            float timer = 0;
            Vector3 curScale = Vector3.one;

            while (timer < ScaleDuration)
            {
                timer += Time.deltaTime;
                curScale = Vector3.Lerp(curScale, Vector3.zero, timer / ScaleDuration);
                selfRectTransform.localScale = curScale;
                await UniTask.Yield();
            }

            selfRectTransform.localScale = Vector3.zero;
            
            bgClickHandler.gameObject.SetActive(false);
        }
    }
}