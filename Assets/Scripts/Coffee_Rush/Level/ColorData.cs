using UnityEngine;

namespace Coffee_Rush.Board
{
    [CreateAssetMenu(fileName = "ColorData", menuName = "Coffee Rush/Level Data/Color Data", order = 1)]
    public class ColorData : ScriptableObject
    {
        public Material[] colorMaterials;
    }
}