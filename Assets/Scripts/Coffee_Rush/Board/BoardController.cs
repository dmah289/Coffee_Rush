using System;
using System.Collections;
using Coffee_Rush.Level;
using Cysharp.Threading.Tasks;
using Framework.DesignPattern;
using UnityEngine;

namespace Coffee_Rush.Board
{
    public class BoardController : MonoSingleton<BoardController>
    {
        [Header("Self References")]
        [SerializeField] public BoardLayoutGenerator layoutGenerator;
        [SerializeField] private BoardObjectSpawner objectSpawner;

        public int BlockCount
        {
            get => objectSpawner.BlockCount;
            set => objectSpawner.BlockCount = value;
        }


        protected override void Awake()
        {
            base.Awake();
            layoutGenerator = GetComponent<BoardLayoutGenerator>();
            objectSpawner = GetComponent<BoardObjectSpawner>();
        }

        public async UniTask EnterLevel(LevelData levelData)
        {
            await layoutGenerator.SetupBoard(levelData);
            await objectSpawner.SpawnObjects(levelData, layoutGenerator.tiles);
        }

        public void ReturnLevelAssetsToPool()
        {
            layoutGenerator.ReturnToPool();
            objectSpawner.ReturnAllObjects();
        }
        
        public void DecreaseBlockCount()
        {
            objectSpawner.BlockCount--;
        }
    }
}