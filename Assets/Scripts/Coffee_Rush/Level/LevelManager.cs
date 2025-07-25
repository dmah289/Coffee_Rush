using System;
using System.Collections;
using BaseSystem;
using Coffee_Rush.Board;
using Framework.DesignPattern;
using Framework.ObjectPooling;
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

        private void OnEnable()
        {
            StartCoroutine(EnterLevel());
        }

        private IEnumerator EnterLevel()
        {
            SelectionController.Instance.gameObject.SetActive(true);
            
            if (!poolingManager.IsInGamePoolingInitialized)
                yield return poolingManager.InitializeObjectInGamePooling();
            
            
            yield return levelLoader.LoadCurrentLevel();
            yield return boardController.EnterLevel(levelLoader.currLevelData);
            levelTimer.Setup(levelLoader.currLevelData.totalTime);
        }

        public void FailLevel()
        {
            loseManager.FailLevel();
            boardController.ResetLevelAssets();
            
            SelectionController.Instance.gameObject.SetActive(false);
        }
    }
}