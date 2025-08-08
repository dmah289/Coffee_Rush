using Coffee_Rush.Level;
using Coffee_Rush.UI.BaseSystem;
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
        public override void ShowPopup()
        {
            if(CanvasManager.Instance.CurPage == ePageType.InGame)
                LevelManager.Instance.StopGameplay();
            
            base.ShowPopup();
        }

        protected override async UniTaskVoid HidePopupAsync()
        {
            if(CanvasManager.Instance.CurPage == ePageType.InGame)
                LevelManager.Instance.ResumeGameplay();
            
            base.HidePopupAsync().Forget();
        }
        
        
    }
}