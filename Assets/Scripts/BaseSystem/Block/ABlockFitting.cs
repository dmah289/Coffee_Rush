using Coffee_Rush.Block;
using Coffee_Rush.Board;
using UnityEngine;

namespace BaseSystem
{
    [RequireComponent(typeof(ABlockController))]
    public abstract class ABlockFitting : MonoBehaviour
    {
        [SerializeField] protected Transform checkPoint;
        [SerializeField] protected Vector3 centerToCheckPointOffset;
        
        public void CalculateCheckPointOffset()
        {
            centerToCheckPointOffset = transform.position - checkPoint.position;
        }
    }
}