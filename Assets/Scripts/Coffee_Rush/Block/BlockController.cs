using System;
using BaseSystem;
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
        
        [Header("Balance Settings")]
        private Vector3 curEulerNotDragging;
        [SerializeField] private Vector3 initEuler;
        [SerializeField] private float dampingFactor;
        [SerializeField] private float tiltSensitivity;
        [SerializeField] private float maxOffset = 10f;
            
        // Balancing Job
        protected BalancingJob balancingJob;
        protected JobHandle balancingJobHandle;
        protected NativeReference<float3> currentEuler;

        

        protected override void Awake()
        {
            base.Awake();
        
            initEuler = selfTransform.eulerAngles;
            blockMatcher = GetComponent<BLockMatcher>();
            blockFitting = GetComponent<BlockFitting>();
            
            blockFitting.CalculateCheckPointOffset();
        }

        public void SetCheckPointToTargetTile(Vector3 targetTilePos)
        {
            blockFitting.SetCheckPointToTargetTile(targetTilePos);
        }
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            StartCoroutine(blockMatcher.TryCollectGateItem(other, colorType, cupHolders));
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            blockMatcher.MatchingAllowed = false;
        }

        protected override void InitializeAllJobs()
        {
            base.InitializeAllJobs();
            
            currentEuler = new(Allocator.Persistent);
            balancingJob = new BalancingJob()
            {
                InitialEuler = initEuler,
                DampingFactor = dampingFactor,
                TiltSensitivity = tiltSensitivity,
                MaxOffset = maxOffset,
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
            
            if(isDragging) selfTransform.eulerAngles = currentEuler.Value;
            else
            {
                curEulerNotDragging = Vector3.Lerp(curEulerNotDragging, initEuler, dampingFactor * Time.deltaTime);
                selfTransform.eulerAngles = curEulerNotDragging;
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
