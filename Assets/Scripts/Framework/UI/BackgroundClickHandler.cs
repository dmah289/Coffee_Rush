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
        
        [SerializeField] private Image image;
        
        public UnityEvent OnBackgroundHiden;


        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject == gameObject)
                HideBackground();
        }

        public void HideBackground()
        {
            image.FadeAlphaToTarget(FadeDuration).Forget();
            OnBackgroundHiden?.Invoke();
        }
        
        public void ShowBackground()
        {
            gameObject.SetActive(true);
            image.SetAlpha(0.4f);
        }
    }
}