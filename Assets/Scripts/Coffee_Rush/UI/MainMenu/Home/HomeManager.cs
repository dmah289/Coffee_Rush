using Coffee_Rush.UI.BaseSystem;
using Coffee_Rush.UI.MainMenu.Home;
using UnityEngine;

namespace Coffee_Rush.UI.MainMenu
{
    public class HomeManager : MonoBehaviour
    {
        public void OnPlayBtnClicked()
        {
            // if (LifeSystem.Instance.CanPlay)
                CanvasManager.Instance.CurPage = ePageType.LoadingLevel;
        }
    }
}