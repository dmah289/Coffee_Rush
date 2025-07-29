using System;
using System.Threading;
using BaseSystem;
using BaseSystem.Block;
using Coffee_Rush.Board;
using Coffee_Rush.JobCalculation;
using Coffee_Rush.Level;
using Cysharp.Threading.Tasks;
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


        public bool CanSelect
        {
            get => blockMatcher.CanSelect && blockVisual.IceCountDown == 0;
        }
        

        protected override void Awake()
        {
            base.Awake();
            blockMatcher = GetComponent<BLockMatcher>();
            blockFitting = GetComponent<BlockFitting>();
            blockVisual = GetComponentInChildren<BlockVisual>();
            cupHolders = GetComponentsInChildren<CupHolder>();
            
            blockMatcher.AllocateGateItemsArray(cupHolders.Length);
            blockMatcher.CanSelect = true;
        }

        public void SetupOnLevelStarted(Vector3 tilePos, BlockData blockData)
        {
            transform.localScale = Vector3.one;
            
            ColorType = blockData.blockColor;
            BlockType = blockData.blockType;
            
            blockVisual.BlockModelEulerAngle = blockData.eulerAngle;
            blockFitting.SetCheckPointToTargetTile(tilePos);
            blockVisual.IceCountDown = blockData.countdownIce;
            movementDirection = blockData.moveableDir;
            blockVisual.ShowDirectionSprite(blockData.moveableDir);
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
            
            base.OnSelect(mousePos);
        }

        public async UniTaskVoid TryCollectGateItems(GateController gate, CancellationTokenSource cts)
        {
            await blockMatcher.TryCollectGateItem(gate, blockType, colorType, cupHolders, cts);
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
            
            if(isDragging) blockVisual.VisualEulerAngle = currentEuler.Value;
            else
            {
                curEulerNotDragging = Vector3.Lerp(curEulerNotDragging, BlockConfig.initEulerModel, BlockConfig.DampingFactor * Time.deltaTime);
                blockVisual.VisualEulerAngle = curEulerNotDragging;
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
        
        public void OnRevokenToPool()
        {
            blockMatcher.PostprocessToPool(blockType);
            
            ObjectPooler.ReturnToPool((PoolingType)(blockType + (byte)PoolingType.BlockType00 - 1), this);
        }
    }
}