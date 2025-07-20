using UnityEngine;

namespace Coffee_Rush.Block
{
    public static class BlockConfig
    {
        #region Balance Settings
        public static readonly float DampingFactor = 5f;
        public static readonly float TiltSensitivity = 10f;
        public static readonly float MaxOffset = 10f;
        public static readonly Vector3 initEulerModel = new (-90f, 0f, 0f);
        #endregion
        
        
        #region Movement Settings
        public static readonly float Speed = 30f;
        #endregion
    }
}