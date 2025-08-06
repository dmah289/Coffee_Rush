using Coffee_Rush.Level;
using Coffee_Rush.UI.BaseSystem;
using Coffee_Rush.UI.MainMenu.Home;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Coffee_Rush.UI.InGame
{
    public class ReplayPopup : APopup
    {
        [SerializeField] private LoadingLevel loadingLevel;
        
        public void OnGiveUpClicked()
        {
            loadingLevel.NextPage = ePageType.InGame;
            
            LevelManager.Instance.ReplayLevelAsync().Forget();
        }
    }
}