using System;
using Coffee_Rush.Block;
using UnityEngine;

namespace Coffee_Rush.Level
{
    [Serializable]
    public struct CellData
    {
        public bool isActive;
        public eBlockType blockType;
    }
    
    [CreateAssetMenu(fileName = "LevelData", menuName = "Coffee Rush/Level Data", order = 1)]
    public class LevelData : ScriptableObject
    {
        public int width;
        public int height;
        public int levelIndex;
        public CellData[] cellsData;

        public CellData GetCellData(int row, int col)
        {
            return cellsData[row * width + col];
        }

        public void SetCellData(int row, int col, CellData cellData)
        {
            cellsData[row * width + col] = cellData;
        }

        public void InitializeCellsDataArray()
        {
            cellsData = new CellData[height * width];
        }
    }
}