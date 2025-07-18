#if UNITY_EDITOR
using BaseSystem;
using LevelEditor.Scripts.LeftSide;
using UnityEngine;

namespace LevelEditor.Scripts.Visualization.Block
{
    public class BlockControllerEdit : ABlockController
    {
        public BlockFittingEdit blockFitting;

        protected override void Awake()
        {
            base.Awake();
            
            blockFitting = GetComponent<BlockFittingEdit>();
            blockFitting.CalculateCheckPointOffset();
        }

        public override void OnSelect(Vector3 mousePos)
        {
            if (BlockTypeSelection.Instance.CanRemove)
            {
                BlockTypeSelection.Instance.Blocks.Remove(this);
                ApplyJobResults();
                gameObject.SetActive(false);
            }
            
            base.OnSelect(mousePos);
        }

        public override void OnDeselect()
        {
            base.OnDeselect();
            
            blockFitting.FitBoard();
        }
    }
}
#endif