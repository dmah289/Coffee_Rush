using System;
using System.Collections;
using BaseSystem;
using Coffee_Rush.Board;
using Cysharp.Threading.Tasks;
using Framework;
using Framework.DesignPattern;
using Framework.ObjectPooling;
using Unity.VisualScripting;
using UnityEngine;

namespace Coffee_Rush.Level
{
    public class LevelManager : MonoSingleton<LevelManager>
    {
        [Header("Self Components")]
        [SerializeField] private LevelLoader levelLoader;
        
        [Header("Manager References")] 
        [SerializeField] public BoardController boardController;
        [SerializeField] private PoolingManager poolingManager;
        [SerializeField] private LoseManager loseManager;
        [SerializeField] private LevelTimer levelTimer;

        private async void OnEnable()
        {
            await EnterLevel();
        }

        private async UniTask EnterLevel()
        {
            SelectionController.Instance.gameObject.SetActive(true);
            
            if (!poolingManager.IsInGamePoolingInitialized)
                await poolingManager.InitializeObjectInGamePooling();
            
            
            await levelLoader.LoadCurrentLevel();
            await boardController.EnterLevel(levelLoader.currLevelData);
            levelTimer.Setup(levelLoader.currLevelData.totalTime);
        }

        public void FailLevel()
        {
            SelectionController.Instance.EndLevel();
            boardController.ResetLevelAssets();
        }

        public async UniTask WinLevel()
        {
            SelectionController.Instance.EndLevel();
            boardController.ResetLevelAssets();
            
            await UniTask.Delay(3000);
            PlayerPrefs.SetInt(KeySave.LevelIndexKey, 
                PlayerPrefs.GetInt(KeySave.LevelIndexKey, 0) + 1);

            await EnterLevel();
        }
    }
}