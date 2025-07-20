using System;
using BaseSystem;
using BaseSystem.Block;
using Coffee_Rush.Board;
using Coffee_Rush.JobCalculation;
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
            
        // Balancing Job
        protected BalancingJob balancingJob;
        protected JobHandle balancingJobHandle;
        protected NativeReference<float3> currentEuler;

        

        protected override void Awake()
        {
            base.Awake();
            blockMatcher = GetComponent<BLockMatcher>();
            blockFitting = GetComponent<BlockFitting>();
            blockVisual = GetComponentInChildren<BlockVisual>();
            
            blockFitting.CalculateCheckPointOffset();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            curEulerNotDragging = BlockConfig.initEulerModel;
            
        }

        public void SetCheckPointToTargetTile(Vector3 targetTilePos)
        {
            blockFitting.SetCheckPointToTargetTile(targetTilePos);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Gate"))
                StartCoroutine(blockMatcher.TryCollectGateItem(other, colorType, cupHolders));
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Gate"))
                blockMatcher.MatchingAllowed = false;
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
            base.ApplyJobResults();
            
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
            
            blockFitting.FitBoard();
        }
    }
}
