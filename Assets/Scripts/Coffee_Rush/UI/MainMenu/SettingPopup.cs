using Cysharp.Threading.Tasks;
using Framework.Extensions;
using Framework.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee_Rush.UI.MainMenu.Home
{
    public class SettingPopup : MonoBehaviour
    {
        [SerializeField] private BackgroundClickHandler bg;
        [SerializeField] private RectTransform selfRectTransform;
        [SerializeField] private Image image;


        private float ScaleDuration = 0.2f;
        
        
        public void ShowPopup()
        {
            bg.ShowBackground();
            selfRectTransform.localScale = Vector3.one;
            image.SetAlpha(1);
        }

        public void HidePopup() => HidePopupAsync().Forget();
        private async UniTaskVoid HidePopupAsync()
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
            
            bg.gameObject.SetActive(false);
        }
    }
}