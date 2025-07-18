#if UNITY_EDITOR
using BaseSystem;
using LevelEditor.Scripts.LeftSide;
using UnityEngine;

namespace LevelEditor.Scripts.Visualization.Block
{
    public class BlockFittingEdit : ABlockFitting
    {
        [SerializeField] private BlockControllerEdit blockControllerEdit;
        [SerializeField] public int row;
        [SerializeField] public int col;

        private void Awake()
        {
            blockControllerEdit = GetComponent<BlockControllerEdit>();
        }

        public void FitBoard()
        {
            Vector3 worldPos = checkPoint.position;
            
            (Vector3 coordPos, int row, int col) res = HeaderLeftEditor.Instance.GetCoordPos(worldPos);

            transform.position = res.coordPos + centerToCheckPointOffset;
            row = res.row;
            col = res.col;
        }
    }
}
#endif