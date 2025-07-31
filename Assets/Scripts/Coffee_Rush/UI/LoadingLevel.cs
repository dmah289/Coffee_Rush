using Coffee_Rush.Level;
using Coffee_Rush.UI.BaseSystem;
using Cysharp.Threading.Tasks;

namespace Coffee_Rush.UI
{
    public class LoadingLevel : ALoadingPage, IPage
    {
        protected override async UniTaskVoid OnFullFillAmount()
        {
            CanvasManager.Instance.CurPage = ePageType.InGame;
            await LevelManager.Instance.EnterLevel();
        }

        public void Show()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                StartLoading();
            }
        }

        public void Hide()
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }
    }
}