#if UNITY_EDITOR
using System;
using Coffee_Rush.Level;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee_Rush.LevelEditor
{
    public class HeaderLeftEditor : MonoBehaviour
    {
        [Header("Inputs")]
        [SerializeField] private BoardConfig boardConfig;
        [SerializeField] private InputField in_levelIndex;
        [SerializeField] private InputField in_height;
        [SerializeField] private InputField in_width;

        [Header("Prefabs")]
        [SerializeField] private TileEdit tileEditPrefab;

        [Header("Settings")]
        public TileEdit[,] cellsEdit;
        public LevelData currLevelData;
        public int width;
        public int height;
        
        public bool IsLevelDataInitialized => currLevelData != null;
        
        private void InitializeLevelDataObject()
        {
            if(!int.TryParse(in_height.text, out height) || !int.TryParse(in_width.text, out width))
                throw new Exception("Invalid input for height or width. Please enter valid integers.");
            
            if (!int.TryParse(in_levelIndex.text, out int levelIndex))
                throw new Exception("Invalid level index input. Please enter a valid integer.");

            currLevelData = ScriptableObject.CreateInstance<LevelData>();
            currLevelData.levelIndex = levelIndex;
            currLevelData.width = width;
            currLevelData.height = height;
            currLevelData.InitializeCellsDataArray();
        }

        public void OnCreateBoardButtonClicked()
        {
            InitializeLevelDataObject();
            
            int halfWidth = width / 2;
            int halfHeight = height / 2;
            
            bool isEvenWidth = (width & 1) == 0;
            bool isEvenHeight = (height & 1) == 0;
            
            float posX, posY;
            
            cellsEdit = new TileEdit[height,width];
            
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    TileEdit tile = Instantiate(tileEditPrefab);
                    
                    if (isEvenWidth) posX = (j - halfWidth + 0.5f) * boardConfig.cellSize;
                    else posX = (j - halfWidth) * boardConfig.cellSize;

                    if (isEvenHeight) posY = (i - halfHeight + 0.5f) * boardConfig.cellSize;
                    else posY = (i - halfHeight) * boardConfig.cellSize;
                    
                    tile.transform.position = new Vector3(posX, posY, 0f);
                    tile.name = $"Cell_{i}_{j}";
                    cellsEdit[i,j] = tile;
                }
            }
        }
    }
}
#endif