using System;
using Cysharp.Threading.Tasks;
using Framework.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework.UI
{
    public class BackgroundClickHandler : MonoBehaviour, IPointerClickHandler
    {
        public static float FadeDuration = 0.2f;
        
        [SerializeField] private Image selfImg;
        
        public UnityEvent OnBackgroundHiden;
        public UnityEvent OnBackgroundShown;

        
        private void Awake()
        {
            selfImg = GetComponent<Image>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject == gameObject)
                HideBackground();
        }

        public void HideBackground()
        {
            selfImg.FadeAlphaToTarget(FadeDuration).Forget();
            OnBackgroundHiden?.Invoke();
        }
        
        public virtual void ShowBackground()
        {
            gameObject.SetActive(true);
            selfImg.SetAlpha(0.4f);
            OnBackgroundShown?.Invoke();
        }
    }
}