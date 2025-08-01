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
        [SerializeField] private float animationDuration = 0.3f; // Animation duration in seconds

        private CancellationTokenSource cts;

        private async void Start()
        {
            await UniTask.DelayFrame(1);
            OnFooterButtonClicked(footerButtons[1]);
        }

        public void OnFooterButtonClicked(FooterButton btn)
        {
            for (int i = 0; i < footerButtons.Length; i++)
            {
                if (footerButtons[i].Equals(btn))
                {
                    cts?.Cancel();
                    cts?.Dispose();
                    cts = new CancellationTokenSource();
                    
                    print(footerButtons[i].gameObject.name);
                    print(footerButtons[i].GetComponent<RectTransform>().anchoredPosition);
                    RectTransform target = footerButtons[i].GetComponent<RectTransform>();
                    Vector2 targetPos = new Vector2(target.anchoredPosition.x, target.anchoredPosition.y + 25);
                    Debug.Log(targetPos);
                    
                    _ = selection.MoveToTarget(targetPos, animationDuration, cts.Token);
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