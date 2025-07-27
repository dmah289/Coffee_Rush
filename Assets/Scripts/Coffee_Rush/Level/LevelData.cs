using System;
using Coffee_Rush.Block;
using Coffee_Rush.Board;
using UnityEngine;

namespace Coffee_Rush.Level
{
    [Serializable]
    public struct KettleData
    {
        public int row;
        public int col;
        public int countdown;
    }
    
    [Serializable]
    public struct BlockerData
    {
        public int row;
        public int col;
        public eBlockType blockerType;
        public eMovementDirection movementDirection;
    }
    
    [Serializable]
    public struct GateData
    {
        public int row;
        public int col;
        public eDirection gateDir;
        public eColorType[] itemColors;
        public CompressedItemPath compressedItemPath;
    }

    [Serializable]
    public struct CompressedItemPath
    {
        public int[] turnIndices;
        public eDirection[] turnDirections;
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
        public eMovementDirection moveableDir;
        public int countdownIce;
    }
    
    [CreateAssetMenu(fileName = "LevelData", menuName = "Coffee Rush/Level Data", order = 1)]
    public class LevelData : ScriptableObject
    {
        public int width;
        public int height;
        public int levelIndex;
        public BlockData[] blocksData;
        public float totalTime;
        public TileData[] cellsData;
        
        public GateData[] gatesData;
        public BlockerData[] blockersData;
        public KettleData[] kettlesData;

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