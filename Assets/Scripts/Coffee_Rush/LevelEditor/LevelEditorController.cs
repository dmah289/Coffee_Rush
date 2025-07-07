#if UNITY_EDITOR
using System;
using Coffee_Rush.Level;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee_Rush.LevelEditor
{
    public class LevelEditorController : MonoBehaviour
    {
        [Header("Inputs")]
        [SerializeField] private BoardConfig boardConfig;
        [SerializeField] private InputField levelIndexInput;
        [SerializeField] private InputField in_height;
        [SerializeField] private InputField in_width;

        [Header("Prefabs")]
        [SerializeField] private CellEdit cellEditPrefab;

        [Header("Settings")]
        private CellEdit[] cells;
        private LevelData currLevelData;
        public int width;
        public int height;

        public void OnExportLevelClicked()
        {
            SaveAllData();
            SaveAsset();
        }

        public void OnCreateBoardButtonClicked()
        {
            InitializeLevelDataObject();
            
            int halfWidth = width / 2;
            int halfHeight = height / 2;
            
            bool isEvenWidth = (width & 1) == 0;
            bool isEvenHeight = (height & 1) == 0;
            
            float posX, posY;
            
            cells = new CellEdit[height * width];
            
            for (int i = 0; i < height; i++)
            {
                
                for (int j = 0; j < width; j++)
                {
                    CellEdit cell = Instantiate(cellEditPrefab, transform);
                    
                    if (isEvenWidth) posX = (j - halfWidth + 0.5f) * boardConfig.cellSize;
                    else posX = (j - halfWidth) * boardConfig.cellSize;

                    if (isEvenHeight) posY = (i - halfHeight + 0.5f) * boardConfig.cellSize;
                    else posY = (i - halfHeight) * boardConfig.cellSize;
                    
                    cell.transform.position = new Vector3(posX, posY, 0f);
                    cell.name = $"Cell_{i}_{j}";
                    cells[i * width + j] = cell;
                }
            }
        }

        private void InitializeLevelDataObject()
        {
            if(!int.TryParse(in_height.text, out height) || !int.TryParse(in_width.text, out width))
            {
                throw new Exception("Invalid input for height or width. Please enter valid integers.");
            }
            
            if (!int.TryParse(levelIndexInput.text, out int levelIndex))
            {
                throw new Exception("Invalid level index input. Please enter a valid integer.");
            }

            currLevelData = new LevelData();
            currLevelData.width = width;
            currLevelData.height = height;
        }

        private void SaveAsset()
        {
            string path = $"Assets/Resources/LevelData/{currLevelData.levelIndex}.asset";
            EditorUtility.SetDirty(currLevelData);
            AssetDatabase.CreateAsset(currLevelData, path);
            AssetDatabase.SaveAssets();
        }

        private void SaveAllData()
        {
            currLevelData.cellsData = new CellData[boardConfig.maxWidth * boardConfig.maxHeight];

            for (int i = 0; i < boardConfig.maxHeight; i++)
            {
                for (int j = 0; j < boardConfig.maxWidth; j++)
                {
                    CellData cellData = new CellData();
                    cellData.isActive = cells[i * boardConfig.maxWidth + j].gameObject.activeSelf;
                    currLevelData.cellsData[i * boardConfig.maxWidth + j] = cellData;
                }
            }
        }
    }
}
#endif