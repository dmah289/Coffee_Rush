using System;
using System.Collections;
using BaseSystem;
using Coffee_Rush.Board;
using Coffee_Rush.UI;
using Coffee_Rush.UI.BaseSystem;
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
        [SerializeField] private LoseManager loseManager;
        [SerializeField] private LevelTimer levelTimer;

        public async UniTask EnterLevel()
        {
            SelectionController.Instance.EnterGameplay();
            
            await levelLoader.LoadCurrentLevel();
            boardController.EnterLevel(levelLoader.currLevelData).Forget();
            levelTimer.Setup(levelLoader.currLevelData.totalTime);
        }

        public async UniTask FailLevel()
        {
            await UniTask.Delay(1000);
            
            SelectionController.Instance.gameObject.SetActive(false);
            boardController.ReturnLevelAssetsToPool();
            levelTimer.PauseTimer();
            
            CanvasManager.Instance.CurPage = ePageType.LoadingLevel;
        }

        public void WinLevel()
        {
            SelectionController.Instance.gameObject.SetActive(false);
            boardController.ReturnLevelAssetsToPool();

            PlayerPrefs.SetInt(KeySave.LevelIndexKey,
                (PlayerPrefs.GetInt(KeySave.LevelIndexKey, 0) + 1) % MaxLevelIndex);
            CanvasManager.Instance.CurPage = ePageType.LoadingLevel;
            
        }

        public async UniTask ReplayLevelAsync()
        {
            await UniTask.Delay(1000);
            
            SelectionController.Instance.gameObject.SetActive(false);
            boardController.ReturnLevelAssetsToPool();
            levelTimer.PauseTimer();
            
            CanvasManager.Instance.CurPage = ePageType.LoadingLevel;
        }
    }
}