using Cysharp.Threading.Tasks;
using Framework.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Coffee_Rush.UI.InGame
{
    public class RestartButton : ScaleAnimButton
    {
        [SerializeField] private eRestartButton btnType;
        public UnityEvent<eRestartButton> OnRestartButtonClicked;

        protected async override UniTaskVoid OnButtonClickedAsync()
        {
            base.OnButtonClickedAsync().Forget();
            
            OnRestartButtonClicked?.Invoke(btnType);
        }
    }
}