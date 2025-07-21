using UnityEngine;

namespace Coffee_Rush.Gate
{
    public static class GateItemConfig
    {
        public static readonly float Distance = 0.8f;
        public static readonly float MoveDuration = 0.2f;
        public static readonly Vector3 WorldRotationOnBlock = new (85f, 0f, 185f);
        public static readonly Vector3 CupLidFloatingPos = new (0, 1, 0);
        public static readonly float PackingDuration = 0.3f;
        public static readonly Vector3[] RotationsByDir =
        {
            new (-110, 0, 0),      // Up
            new (-90, 0, -30),     // Right
            new (-70, 0, 0),       // Down
            new (-90, 0, 30)       // Left
        };
        public static readonly Vector3[] FirstItemSpawnedPosByDir =
        {
            new (1, -1, 0),      // Up
            new (0.7f, -1, 0),      // Right
            new (1, -1, -0.3f),     // Down
            new (1.3f, -1, 0)      // Left
        };
    }
}