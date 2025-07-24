using System;
using BaseSystem;
using BaseSystem.Block;
using Coffee_Rush.Board;
using Coffee_Rush.JobCalculation;
using Coffee_Rush.Level;
using DG.Tweening;
using Framework.ObjectPooling;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Coffee_Rush.Block
{
    public class BlockController : ABlockController
    {
        [Header("Child Components")]
        [SerializeField] private BLockMatcher blockMatcher;
        [SerializeField] protected BlockFitting blockFitting;
        [SerializeField] private BlockVisual blockVisual;
        
        [Header("Balance Settings")]
        private Vector3 curEulerNotDragging;
        
        [Header("Movement Direction")]
        [SerializeField] private eMovementDirection movementDirection;
            
        // Balancing Job
        protected BalancingJob balancingJob;
        protected JobHandle balancingJobHandle;
        protected NativeReference<float3> currentEuler;

        [SerializeField] private int noCollision;
        

        protected override void Awake()
        {
            base.Awake();
            blockMatcher = GetComponent<BLockMatcher>();
            blockFitting = GetComponent<BlockFitting>();
            blockVisual = GetComponentInChildren<BlockVisual>();
            
            blockFitting.CalculateCheckPointOffset();
            blockMatcher.AllocateGateItemsArray(cupHolders.Length);
            blockMatcher.CanSelect = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            curEulerNotDragging = BlockConfig.initEulerModel;
            BLockMatcher.OnBlockFullSlot += blockVisual.OnBlockColected;
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            BLockMatcher.OnBlockFullSlot -= blockVisual.OnBlockColected;
        }

        public override void OnSelect(Vector3 mousePos)
        {
            if (!blockMatcher.CanSelect)
             return;
            
            DOTween.Kill(gameObject);
            transform.DOMoveZ(-1f, 0.2f);
            selfRb.isKinematic = false;
            
            SetupJobConfigs(mousePos);
        }

        public void SetCheckPointToTargetTile(Vector3 targetTilePos)
        {
            blockFitting.SetCheckPointToTargetTile(targetTilePos);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            noCollision++;
            if (other.gameObject.CompareTag("Gate") && noCollision == 1)
            {
                StartCoroutine(blockMatcher.TryCollectGateItem(other, blockType, colorType, cupHolders));
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Gate"))
            {
                noCollision--;
                if(noCollision == 0)
                    blockMatcher.MatchingAllowed = false;
            }
        }

        protected override void InitializeAllJobs()
        {
            base.InitializeAllJobs();
            
            currentEuler = new(Allocator.Persistent);
            currentEuler.Value = BlockConfig.initEulerModel;
            balancingJob = new BalancingJob()
            {
                InitialEuler = BlockConfig.initEulerModel,
                DampingFactor = BlockConfig.DampingFactor,
                TiltSensitivity = BlockConfig.TiltSensitivity,
                MaxOffset = BlockConfig.MaxOffset,
                CurrentEuler = currentEuler
            };
        }

        protected override void ScheduleAllJobs(Vector3 currTouchPos)
        {
            base.ScheduleAllJobs(currTouchPos);
            
            Vector3 currBlockPos = selfTransform.position;
            currBlockPos.z = 0f;
            balancingJob.CurBlockPos = currBlockPos;
            balancingJob.CenterToInitTouchOffset = centerToTouchOffset;
            balancingJob.CurTouchPos = currTouchPos;
            balancingJob.IsDragging = isDragging;
            balancingJob.DeltaTime = Time.deltaTime;
            balancingJobHandle = balancingJob.Schedule(velocityCalculationJobHandle);
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
            
            balancingJobHandle.Complete();
            
            if(isDragging) blockVisual.EulerRotation = currentEuler.Value;
            else
            {
                curEulerNotDragging = Vector3.Lerp(curEulerNotDragging, BlockConfig.initEulerModel, BlockConfig.DampingFactor * Time.deltaTime);
                blockVisual.EulerRotation = curEulerNotDragging;
            }
        }

        protected override void ReleaseNativeMemory()
        {
            base.ReleaseNativeMemory();
            
            if (currentEuler.IsCreated) currentEuler.Dispose();
        }

        public override void OnDeselect()
        {
            base.OnDeselect();
            if(blockMatcher.CanSelect) blockFitting.FitBoard();
        }

        public void SetMovementDirection(eMovementDirection moveableDir)
        {
            movementDirection = moveableDir;
            blockVisual.ShowDirectionSprite(moveableDir);
        }

        public void SetBlockObstacle(int iceCountdown)
        {
            blockVisual.IceCountDown = iceCountdown;
        }

        public void OnRevokenToPool()
        {
            blockMatcher.PostprocessToPool(blockType);
            
            ObjectPooler.ReturnToPool((PoolingType)(blockType + (byte)PoolingType.BlockType00 - 1), this);
        }
    }
}
