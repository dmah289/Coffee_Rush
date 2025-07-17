using System;
using System.Collections;
using Coffee_Rush.Board;
using Framework.ObjectPooling;
using UnityEngine;

namespace Coffee_Rush.Level
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }
        
        [Header("References")] 
        [SerializeField] private LevelLoader levelLoader;
        [SerializeField] public BoardController boardController;
        [SerializeField] private PoolingManager poolingManager;
        
        public GateController gateController;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

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
            
            // gateController.Setup();
        }
    }
}