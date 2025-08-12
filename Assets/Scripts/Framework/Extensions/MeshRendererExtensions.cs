using Coffee_Rush.Board;
using UnityEngine;

namespace Framework.Extensions
{
    public static class MeshRendererExtensions
    {
        private static readonly int MainTexSt = Shader.PropertyToID("_MainTex_ST");
        private static MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        private const float ColorTextureHorizontalOffset = 340 / 2048f;
        private const float ColorTextureVerticalOffset = 428 / 2048f;
        private const int elementsPerRow = 6;
        
        public static void SetTextureOffsetByColor(this MeshRenderer meshRenderer, eColorType colorType)
        {
            meshRenderer.GetPropertyBlock(mpb);

            if (colorType == eColorType.None)
            {
                mpb.SetVector(MainTexSt, new Vector4(1, 1, 0, ColorTextureHorizontalOffset));
                meshRenderer.SetPropertyBlock(mpb);
                return;
            }
            
            int colorIdx = (byte)colorType - 1;
            float offsetX = ColorTextureHorizontalOffset * (colorIdx % elementsPerRow);
            float offsetY = -ColorTextureVerticalOffset * (colorIdx / elementsPerRow);
            
            mpb.SetVector(MainTexSt, new Vector4(1, 1, offsetX, offsetY));
            meshRenderer.SetPropertyBlock(mpb);
        }
    }
}