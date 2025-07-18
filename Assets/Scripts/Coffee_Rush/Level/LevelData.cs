using System;
using Coffee_Rush.Block;
using Coffee_Rush.Board;
using UnityEngine;

namespace Coffee_Rush.Level
{
    [Serializable]
    public struct GateData
    {
        public int row;
        public int col;
        public eDirection gateDir;
        public eColorType[] itemColors;
    }
    
    [Serializable]
    public struct TileData
    {
        public bool isActive;
        public eBlockType blockType;
        public eColorType blockColor;
    }

    [Serializable]
    public struct BlockData
    {
        public int row;
        public int col;
        public eBlockType blockType;
        public eColorType blockColor;
    }
    
    [CreateAssetMenu(fileName = "LevelData", menuName = "Coffee Rush/Level Data", order = 1)]
    public class LevelData : ScriptableObject
    {
        public int width;
        public int height;
        public int levelIndex;
        public TileData[] cellsData;
        public BlockData[] blocksData;
        public GateData[] gatesData;

        public TileData GetCellData(int row, int col)
        {
            return cellsData[row * width + col];
        }

        public void SetCellData(int row, int col, TileData tileData)
        {
            cellsData[row * width + col] = tileData;
        }

        public void InitializeCellsDataArray()
        {
            cellsData = new TileData[height * width];
        }
    }
}