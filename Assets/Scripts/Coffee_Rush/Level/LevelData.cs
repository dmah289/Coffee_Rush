using System;
using UnityEngine;

namespace Coffee_Rush.Level
{
    [CreateAssetMenu(fileName = "DataLevel", menuName = "Data/DataLevel", order = 1)]
    public class LevelData : ScriptableObject
    {
        public int levelIndex;
        public int width;
        public int height;
        public CellData[] cells;
        
        
        public LevelData(int levelIndex, int width, int height)
        {
            if(width <= 0 || height <= 0) 
                throw new ArgumentOutOfRangeException("Matrix dimensions must be greater than zero");
            
            this.levelIndex = levelIndex;
            this.width = width;
            this.height = height;
            cells = new CellData[width * height];
        }
    }
}