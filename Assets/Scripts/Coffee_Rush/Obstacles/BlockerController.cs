using BaseSystem;
using BaseSystem.Block;
using Coffee_Rush.Block;
using Coffee_Rush.Level;
using Framework.ObjectPooling;
using UnityEngine;

namespace Coffee_Rush.Obstacles
{
    [RequireComponent(typeof(BlockFitting))]
    public class BlockerController : ABlockController, IPooledObject
    {
        [Header("Movement Direction")]
        [SerializeField] private SpriteRenderer verticalSprite;
        [SerializeField] private SpriteRenderer horizontalSprite;
        [SerializeField] private eMovementDirection movementDirection;
        
        
        [Header("Self Components")]
        [SerializeField] private BlockFitting blockFitting;

        protected override void Awake()
        {
            base.Awake();
            
            blockFitting = GetComponent<BlockFitting>();
        }

        public void Setup(Vector3 position, eMovementDirection direction)
        {
            SetupMovementDirection(direction);
            blockFitting.SetCheckPointToTargetTile(position);
        }
        
        protected override void ApplyJobResults()
        {
            velocityCalculationJobHandle.Complete();

            if (isDragging)
            {
                Vector3 res = finalVelocity.Value;
                if (movementDirection == eMovementDirection.Horizontal) res.y = 0;
                else if(movementDirection == eMovementDirection.Vertical) res.x = 0;
                selfRb.velocity = res;
            }
        }
        
        public override void OnDeselect()
        {
            base.OnDeselect();
            
            blockFitting.FitBoard();
        }

        private void SetupMovementDirection(eMovementDirection direction)
        {
            movementDirection = direction;
            
            if (direction == eMovementDirection.Both)
            {
                horizontalSprite.gameObject.SetActive(false);
                verticalSprite.gameObject.SetActive(false);
            }
            else if (direction == eMovementDirection.Horizontal)
            {
                horizontalSprite.gameObject.SetActive(true);
                verticalSprite.gameObject.SetActive(false);
            }
            else if (direction == eMovementDirection.Vertical)
            {
                horizontalSprite.gameObject.SetActive(false);
                verticalSprite.gameObject.SetActive(true);
            }
        }

        public void OnRevoke()
        {
            ObjectPooler.ReturnToPool((PoolingType)(blockType + (byte)PoolingType.BlockerType00 - 1), this);
        }
    }
}