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
        [Header("UI Elements")]
        [SerializeField] protected BackgroundClickHandler bgClickHandler;
        [SerializeField] protected RectTransform selfRectTransform;
        [SerializeField] protected Image popUpImg;


        [Header("Scale Settings")]
        [SerializeField] protected float ScaleDuration = 0.2f;
        [SerializeField] protected Vector3 targetScale;

        private void Awake()
        {
            bgClickHandler = GetComponentInParent<BackgroundClickHandler>();
            selfRectTransform = GetComponent<RectTransform>();
            popUpImg = GetComponent<Image>();
        }

        public virtual void ShowPopup()
        {
            selfRectTransform.localScale = Vector3.one;
            popUpImg.SetAlpha(1);
        }

        public void HidePopup() => HidePopupAsync().Forget();
        protected virtual async UniTaskVoid HidePopupAsync()
        {
            popUpImg.FadeAlphaToTarget(0.9f, ScaleDuration).Forget();
            
            // float timer = 0;
            // Vector3 curScale = Vector3.one;
            //
            // while (timer < ScaleDuration)
            // {
            //     timer += Time.deltaTime;
            //     curScale = Vector3.Lerp(curScale, targetScale, timer / ScaleDuration);
            //     selfRectTransform.localScale = curScale;
            //     await UniTask.Yield();
            // }
            //
            // selfRectTransform.localScale = targetScale;
            
            await UniTask.Delay(TimeSpan.FromSeconds(ScaleDuration));
            
            bgClickHandler.gameObject.SetActive(false);
        }
    }
}