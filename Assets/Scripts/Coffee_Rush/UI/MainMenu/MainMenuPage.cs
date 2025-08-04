using System;
using Coffee_Rush.UI.BaseSystem;
using DG.Tweening;
using UnityEngine;

namespace Coffee_Rush.UI.MainMenu
{
    public class MainMenuPage : MonoBehaviour, IPage
    {
        [SerializeField] private RectTransform tabsParent;

        private void Awake()
        {
            OnFooterButtonClicked(1);
        }

        // TODO : Register callback when the footer button is clicked
        public void OnFooterButtonClicked(int index)
        {
            tabsParent.DOAnchorMax(new Vector2(2-index, 1), 0.1f);
            tabsParent.DOAnchorMin(new Vector2(1-index, 0), 0.1f);
        }
        
        public void Show()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
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