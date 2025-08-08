using System;
using System.Collections;
using BaseSystem;
using Coffee_Rush.Board;
using Coffee_Rush.UI;
using Coffee_Rush.UI.BaseSystem;
using Coffee_Rush.UI.InGame;
using Cysharp.Threading.Tasks;
using Framework;
using Framework.DesignPattern;
using Framework.ObjectPooling;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Coffee_Rush.Level
{
    public class LevelManager : MonoSingleton<LevelManager>
    {
        private static int MaxLevelIndex = 3;
        
        [Header("Self Components")]
        [SerializeField] public LevelLoader levelLoader;
        
        [Header("Manager References")]
        [SerializeField] public BoardController boardController;
        [SerializeField] private PoolingManager poolingManager;
        [SerializeField] private LevelTimer levelTimer;
        [SerializeField] private LoadingLevel loadingLevel;
        [SerializeField] private LoosePanel loosePanel;

        public async UniTask EnterGameplay()
        {
            SelectionController.Instance.EnterGameplay();
            
            await levelLoader.LoadCurrentLevel();
            boardController.EnterLevel(levelLoader.currLevelData).Forget();
            levelTimer.Setup(levelLoader.currLevelData.totalTime);
        }

        public void StopGameplay()
        {
            SelectionController.Instance.gameObject.SetActive(false);
            levelTimer.PauseTimer();
        }

        public void ResumeGameplay()
        {
            SelectionController.Instance.gameObject.SetActive(true);
            levelTimer.ResumeTimer();
        }

        public async UniTask FailLevel()
        {
            await UniTask.Delay(500);
            
            boardController.ReturnLevelAssetsToPool();
            
            loadingLevel.NextPage = ePageType.MainMenu;
            CanvasManager.Instance.CurPage = ePageType.LoadingLevel;
        }

        public async UniTask WinLevel()
        {
            await UniTask.Delay(500);
            
            boardController.ReturnLevelAssetsToPool();

            PlayerPrefs.SetInt(KeySave.LevelIndexKey,
                (PlayerPrefs.GetInt(KeySave.LevelIndexKey, 0) + 1) % MaxLevelIndex);
            
            loadingLevel.NextPage = ePageType.InGame;
            CanvasManager.Instance.CurPage = ePageType.LoadingLevel;
            
        }

        public async UniTask ReplayLevelAsync()
        {
            await UniTask.Delay(500);
            
            boardController.ReturnLevelAssetsToPool();
            
            loadingLevel.NextPage = ePageType.InGame;
            CanvasManager.Instance.CurPage = ePageType.LoadingLevel;
        }

        public void ShowLoosePanel(eLooseReason reason)
        {
            loosePanel.Show(reason).Forget();
        }
    }
}