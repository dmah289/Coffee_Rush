using System;
using Framework.Extensions;
using UnityEngine;

namespace Coffee_Rush.Board
{
    public class GateItem : MonoBehaviour
    {
        [Header("Render Components")]
        [SerializeField] MeshRenderer visualMeshRenderer;

        private eColorType colorType;
        public eColorType ColorType
        {
            get => colorType;
            set
            {
                if(colorType != value)
                {
                    colorType = value;
                    visualMeshRenderer.SetTextureOffsetByColor(colorType);
                }
            }
        }
    }
}