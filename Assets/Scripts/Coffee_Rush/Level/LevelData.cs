using System;
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
    }
}