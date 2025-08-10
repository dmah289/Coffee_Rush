using Coffee_Rush.Level;
using Coffee_Rush.UI.BaseSystem;
using Coffee_Rush.UI.MainMenu.Home;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Coffee_Rush.UI.InGame
{
    public enum eLooseReason
    {
        TimeOut,
        KettleExplosion
    }
    
    public class LoosePanel : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private RectTransform selfRect;
        
        [Header("Panels")]
        [SerializeField] private GameObject revievePanel;
        [SerializeField] private RectTransform failLevelPanel;
        
        [Header("Reference")]
        [SerializeField] private LevelTimer levelTimer;

        public async UniTaskVoid Show(eLooseReason reason)
        {
            LevelManager.Instance.StopGameplay();

            await UniTask.Delay(500);
            
            gameObject.SetActive(true);
            
            revievePanel.SetActive(reason == eLooseReason.TimeOut);
            failLevelPanel.gameObject.SetActive(reason != eLooseReason.TimeOut);
            
            selfRect.localScale = Vector3.zero;
            selfRect.DOScale(Vector3.one, 0.2f);
        }

        public void OnNoBtnClicked()
        {
            revievePanel.SetActive(false);
            failLevelPanel.gameObject.SetActive(true);
            
            failLevelPanel.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            failLevelPanel.DOScale(Vector3.one, 0.2f);
        }

        public void OnContinueBtnClicked()
        {
            LifeSystem.Instance.CurLife--;
            LevelManager.Instance.FailLevel().Forget();
            gameObject.SetActive(false);
        }
    }
}