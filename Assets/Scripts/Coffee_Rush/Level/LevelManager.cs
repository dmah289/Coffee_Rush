using System;
using System.Collections;
using Coffee_Rush.Board;
using Framework.ObjectPooling;
using UnityEngine;

namespace Coffee_Rush.Level
{
    public class LevelManager : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private LevelLoader levelLoader;
        [SerializeField] private BoardController boardController;
        [SerializeField] private PoolingManager poolingManager;
        

        private void OnEnable()
        {
            StartCoroutine(EnterLevel());
        }

        private IEnumerator EnterLevel()
        {
            if (!poolingManager.IsInGamePoolingInitialized)
                yield return poolingManager.InitializeObjectInGamePooling();
            
            yield return levelLoader.LoadCurrentLevel();
            
            boardController.EnterLevel(levelLoader.currLevelData);
        }
    }
}