using System;
using Coffee_Rush.UI.BaseSystem;
using Coffee_Rush.UI.MainMenu.Home;
using Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee_Rush.UI.MainMenu
{
    public class HomeManager : MonoBehaviour
    {
        [Header("Level Path")]
        [SerializeField] private Text[] levelIndexTexts;
        
        public void OnPlayBtnClicked()
        {
            // if (LifeSystem.Instance.CanPlay)
                CanvasManager.Instance.CurPage = ePageType.LoadingLevel;
        }

        private void OnEnable()
        {
            SetupLevelPath();
        }

        private void SetupLevelPath()
        {
            int curLevelIndex = PlayerPrefs.GetInt(KeySave.LevelIndexKey, 0);
            for (int i = 0; i < levelIndexTexts.Length; i++)
            {
                levelIndexTexts[i].text = $"{curLevelIndex + i}";
            }
        }
    }
}