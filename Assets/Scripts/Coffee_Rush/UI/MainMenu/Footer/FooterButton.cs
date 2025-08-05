using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Framework.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee_Rush.UI.MainMenu.Footer
{
    public class FooterButton : MonoBehaviour
    {
        public static Vector3 minTitleButtonScale = new (0.5f, 0.5f, 0.5f);
        
        [SerializeField] private RectTransform iconRect;
        [SerializeField] private Text title;
        [SerializeField] private float selfRatio;

        private Vector2 initPos, targetPos;

        private void Awake()
        {
            initPos = iconRect.anchoredPosition;
            targetPos = new Vector2(iconRect.anchoredPosition.x, iconRect.anchoredPosition.y + 45);
        }

        public void OnLerpRatioChanged(float ratio)
        {
            iconRect.anchoredPosition = Vector2.Lerp(initPos, targetPos, 1 - Mathf.Abs(ratio - selfRatio) * 2);
            float alphaText = Mathf.Lerp(0.5f, 1, 1 - Mathf.Abs(ratio - selfRatio) * 4);
            
            if(Mathf.Approximately(alphaText, 0.5f)) title.gameObject.SetActive(false);
            else title.gameObject.SetActive(true);
            
            title.SetAlpha(alphaText);
            title.GetComponent<RectTransform>().localScale = Vector3.Lerp(minTitleButtonScale, Vector3.one, alphaText);

        }
    }
}