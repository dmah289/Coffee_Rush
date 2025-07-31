using Coffee_Rush.UI.BaseSystem;
using UnityEngine;

namespace Coffee_Rush.UI.MainMenu
{
    public class HomeManager : MonoBehaviour
    {
        public void OnPlayBtnClicked()
        {
            CanvasManager.Instance.CurPage = ePageType.LoadingLevel;
        }
    }
}