using UnityEngine;

namespace Coffee_Rush.Board
{
    public static class BoardConfig
    {
        #region Layout Config
        public static readonly Vector2Int[] tileDirections =
        {
            new (0, -1),        // left
            new (1, 0),         // top
            new (0, 1),         // right
            new (-1, 0)         // bottom
        };
        public static readonly float cellSize = 2;
        public static readonly  float borderSize = 0.3f;
        #endregion
    }
}