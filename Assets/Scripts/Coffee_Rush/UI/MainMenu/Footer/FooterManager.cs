using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Framework.Extensions;
using UnityEngine;

namespace Coffee_Rush.UI.MainMenu.Footer
{
    public class FooterManager : MonoBehaviour
    {
        [SerializeField] private RectTransform selection;
        [SerializeField] private FooterButton[] footerButtons;
        [SerializeField] private float animationDuration = 0.3f;

        private CancellationTokenSource cts;

        private async void Start()
        {
            await UniTask.DelayFrame(1);
            OnFooterButtonClicked(footerButtons[1]);
        }

        // TODO : Receive an index of the button instead of the button itself
        public void OnFooterButtonClicked(FooterButton btn)
        {
            for (int i = 0; i < footerButtons.Length; i++)
            {
                if (footerButtons[i].Equals(btn))
                {
                    cts?.Cancel();
                    cts?.Dispose();
                    cts = new CancellationTokenSource();
                    
                    RectTransform target = footerButtons[i].GetComponent<RectTransform>();
                    Vector2 targetPos = new Vector2(target.anchoredPosition.x, target.anchoredPosition.y + 25);
                    
                    _ = selection.MoveToTargetBySpeed(targetPos, 2800, cts.Token);
                    footerButtons[i].OnSelected();
                }
                else footerButtons[i].OnDeselected();
            }
        }

        private void OnDisable()
        {
            cts?.Cancel();
            cts?.Dispose();
        }
    }
}