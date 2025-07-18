#if UNITY_EDITOR
using System;
using Coffee_Rush;
using Coffee_Rush.Block;
using Coffee_Rush.Board;
using Coffee_Rush.Level;
using Coffee_Rush.LevelEditor;
using Framework.DesignPattern;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor.Scripts.LeftSide
{
    public class HeaderLeftEditor : MonoSingleton<HeaderLeftEditor>
    {
        [Header("Board Config")]
        [SerializeField] private Vector2Int[] tileDirections;     // left, top, right, bottom
        [SerializeField] private float cellSize;                  // 2
        [SerializeField] private float borderSize;                // 0.3
        
        [Header("Inputs")]
        [SerializeField] private InputField in_levelIndex;
        [SerializeField] private InputField in_height;
        [SerializeField] private InputField in_width;

        
        [Header("Settings")]
        [SerializeField] private TileEdit tileEditPrefab;
        public TileEdit[,] tilesEdit;
        public Transform tilesParent;
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
            
            tilesEdit = new TileEdit[height,width];
            
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    TileEdit tile = Instantiate(tileEditPrefab, tilesParent);
                    
                    if (isEvenWidth) posX = (j - halfWidth + 0.5f) * cellSize;
                    else posX = (j - halfWidth) * cellSize;

                    if (isEvenHeight) posY = (i - halfHeight + 0.5f) * cellSize;
                    else posY = (i - halfHeight) * cellSize;
                    
                    tile.transform.position = new Vector3(posX, posY, 0f);
                    tile.name = $"Cell_{i}_{j}";
                    tilesEdit[i,j] = tile;
                }
            }
        }
        
        public (Vector3 coordPos, int row, int col) GetCoordPos(Vector3 worldPos)
        {
            int halfWidth = currLevelData.width / 2;
            int halfHeight = currLevelData.height / 2;
    
            bool isEvenWidth = (currLevelData.width & 1) == 0;
            bool isEvenHeight = (currLevelData.height & 1) == 0;
    
            float column, row;
    
            if (isEvenWidth) column = (worldPos.x / cellSize) + halfWidth - 0.5f;
            else column = (worldPos.x / cellSize) + halfWidth;
    
            if (isEvenHeight) row = (worldPos.y / cellSize) + halfHeight - 0.5f;
            else row = (worldPos.y / cellSize) + halfHeight;
            
            int roundedColumn = Mathf.RoundToInt(column);
            int roundedRow = Mathf.RoundToInt(row);
            
            if(roundedRow < 0 || roundedRow >= currLevelData.height || 
               roundedColumn < 0 || roundedColumn >= currLevelData.width)
            {
                return (worldPos, -1, -1);
            }
            
            return (tilesEdit[roundedRow, roundedColumn].transform.position, roundedRow, roundedColumn);
        }
    }
}
#endif