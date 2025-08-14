using UnityEngine;

namespace Coffee_Rush.Block
{
    public static class BlockConfig
    {
        #region Balance Settings
        public static readonly float DampingFactor = 10f;
        public static readonly float TiltSensitivity = 10f;
        public static readonly float MaxOffset = 5f;
        public static readonly Vector3 initEulerModel = new (-90f, 0f, 0f);
        #endregion
        
        #region Movement Settings
        public static readonly float SpeedOnBoard = 40f;
        public static readonly float SnappingSpeed = 10f;
        public static readonly float SpeedToMoveOutOfView = 20f;
        public static readonly float TargetScaleToMove = 1.1f;
        #endregion
    }
}