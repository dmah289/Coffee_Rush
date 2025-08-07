using Coffee_Rush.UI.MainMenu.Home;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Framework.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee_Rush.UI.MainMenu
{
    public class SettingPopup : APopup
    {
        [Header("Movement Settings")]
        [SerializeField] private Vector2 hidenPos;
        [SerializeField] private Vector2 shownPos;
        [SerializeField] private float moveDuration;
        
        [SerializeField] private Ease easeType;
        [SerializeField] private float amplitude;
        [SerializeField] private float period;
        
        
        public override void ShowPopup()
        {
            base.ShowPopup();

            selfRectTransform.DOKill();
            selfRectTransform.anchoredPosition = hidenPos;
            selfRectTransform.DOAnchorPos(shownPos, moveDuration)
                .SetEase(easeType, amplitude, period);
        }

        protected override async UniTaskVoid HidePopupAsync()
        {
            base.HidePopupAsync().Forget();
            
            selfRectTransform.DOKill();
            selfRectTransform.DOAnchorPos(hidenPos, moveDuration)
                .SetEase(Ease.Linear);
        }
        
        
    }
}