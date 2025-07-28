using UnityEngine;

namespace Coffee_Rush.Gate
{
    public static class GateConfig
    {
        public static readonly float GateWidth = 1f;
        public static readonly int[] GateZRotByDir = { -180, 90, 0, -90 };
        public static readonly Vector3 impulseOffset = new (0, 0, 0.5f);
        public static readonly Vector2Int[] GateFitTileDir =
        {
            new (1, 1),       // Up
            new (1, -1),        // Right
            new (-1, -1),       // Down
            new (-1, 1)       // Left
        };
    }
}