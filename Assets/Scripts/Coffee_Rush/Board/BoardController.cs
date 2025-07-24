using System;
using System.Collections;
using Coffee_Rush.Level;
using UnityEngine;

namespace Coffee_Rush.Board
{
    public class BoardController : MonoBehaviour
    {
        [Header("Self References")]
        [SerializeField] public BoardLayoutGenerator layoutGenerator;
        [SerializeField] private BoardObjectSpawner objectSpawner;

        private void Awake()
        {
            layoutGenerator = GetComponent<BoardLayoutGenerator>();
            objectSpawner = GetComponent<BoardObjectSpawner>();
        }

        public IEnumerator EnterLevel(LevelData levelData)
        {
            yield return layoutGenerator.SetupBoard(levelData);
            yield return objectSpawner.SpawnObjects(levelData, layoutGenerator.tiles);
        }

        public void ResetLevelAssets()
        {
            layoutGenerator.RevokeBoard();
            objectSpawner.RevokeObjects();
        }
    }
}