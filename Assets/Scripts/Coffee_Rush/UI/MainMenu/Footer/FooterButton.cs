using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Framework.Extensions;
using UnityEngine;

namespace Coffee_Rush.UI.MainMenu.Footer
{
    public class FooterButton : MonoBehaviour
    {
        [SerializeField] private RectTransform iconRect;
        [SerializeField] private GameObject title;
        
        [SerializeField] private float yOffset = 30;
        
        private CancellationTokenSource cts;

        private Vector2 initPos, targetPos;

        private void Awake()
        {
            initPos = iconRect.anchoredPosition;
            targetPos = new Vector2(iconRect.anchoredPosition.x, iconRect.anchoredPosition.y + 45);
        }

        public void OnSelected()
        {
            if(!title.gameObject.activeSelf)
            {
                cts?.Cancel();
                cts?.Dispose();
                cts = new CancellationTokenSource();
            
                iconRect.MoveToTarget(targetPos, 0.1f, cts.Token).Forget();
            
                title.SetActive(true);
            }
        }

        public void OnDeselected()
        {
            if (title.gameObject.activeSelf)
            {
                cts?.Cancel();
                cts?.Dispose();
                cts = new CancellationTokenSource();
            
                iconRect.MoveToTarget(initPos, 0.1f, cts.Token).Forget();
            
                title.SetActive(false);
            }
        }
    }
}