using Coffee_Rush.Board;
using UnityEngine;

namespace Framework.Extensions
{
    public static class MeshRendererExtensions
    {
        private static readonly int MainTexSt = Shader.PropertyToID("_MainTex_ST");
        private static MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        private static float ColorTextureHorizontalOffset = 340 / 2048f;
        private static float ColorTextureVerticalOffset = 428 / 2048f;
        private static int noElePerRow = 6;
        
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
            float offsetX = ColorTextureHorizontalOffset * (colorIdx % noElePerRow);
            float offsetY = ColorTextureVerticalOffset * (colorIdx / noElePerRow);
            
            mpb.SetVector(MainTexSt, new Vector4(1, 1, offsetX, offsetY));
            meshRenderer.SetPropertyBlock(mpb);
        }
    }
}