using System;
using Coffee_Rush.Block;
using Coffee_Rush.Board;
using Coffee_Rush.JobCalculation;
using DG.Tweening;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace BaseSystem
{
    public abstract class ABlockController : MonoBehaviour, ISelectable
    {
        [Header("Self Components")]
        [SerializeField] protected Transform selfTransform;
        [SerializeField] protected Rigidbody2D selfRb;
        
        [Header("Attributes")]
        [SerializeField] protected eBlockType blockType;
        
        [Header("Movement Settings")]
        [SerializeField] protected Vector3 centerToTouchOffset;
        [SerializeField] protected float speed;
        [SerializeField] protected bool isDragging;
        
        //Velocity Calculation Job
        protected VelocityCalculationJob velocityCalculationJob;
        protected JobHandle velocityCalculationJobHandle;
        protected NativeReference<float3> finalVelocity;
        protected NativeReference<float3> currInitTouchPos;
        

        protected virtual void OnBalance(bool isDragging, Vector3 currTouchPos, Vector3 curLocalTouchPos){}
        

        protected virtual void Awake()
        {
            selfTransform = transform;
            selfRb = selfTransform.GetComponent<Rigidbody2D>();

            selfRb.isKinematic = true;
            speed = 30;
        }

        private void OnEnable()
        {
            InitializeAllJobs();
        }

        protected virtual void InitializeAllJobs()
        {
            finalVelocity = new(Allocator.Persistent);
            currInitTouchPos = new (Allocator.Persistent);
            velocityCalculationJob = new VelocityCalculationJob
            {
                Speed = speed,
                CurInitTouchPos = currInitTouchPos,
                Velocity = finalVelocity
            };
        }

        public void OnSelect(Vector3 mousePos)
        {
            transform.DOMoveZ(-1f, 0.2f);
            selfRb.isKinematic = false;
            
            SetupJobConfigs(mousePos);
        }

        protected virtual void SetupJobConfigs(Vector3 mousePos)
        {
            centerToTouchOffset = mousePos - selfTransform.position;
            centerToTouchOffset.z = 0;
            velocityCalculationJob.CenterToInitTouchOffset = centerToTouchOffset;
        }

        public void OnDrag(Vector3 currTouchPos)
        {
            isDragging = true;
            
            ScheduleAllJobs(currTouchPos);
            
            JobHandle.ScheduleBatchedJobs();
        }

        protected virtual void ScheduleAllJobs(Vector3 currTouchPos)
        {
            velocityCalculationJob.CurTouchPos = currTouchPos;
            Vector3 currBlockPos = selfTransform.position;
            currBlockPos.z = 0f;
            velocityCalculationJob.CurBlockPos = currBlockPos;
            velocityCalculationJobHandle = velocityCalculationJob.Schedule();
        }

        private void LateUpdate()
        {
            ApplyJobResults();
        }

        protected virtual void ApplyJobResults()
        {
            velocityCalculationJobHandle.Complete();

            if (isDragging)
            {
                Vector3 res = finalVelocity.Value;
                selfRb.velocity = res;
            }
        }

        public void OnDeselect()
        {
            transform.DOMoveZ(0f, 0.2f);
            isDragging = false;
            velocityCalculationJobHandle.Complete();
            
            selfRb.velocity = Vector2.zero;
            selfRb.isKinematic = true;
        }

        private void OnDisable()
        {
            ReleaseNativeMemory();
        }

        protected virtual void ReleaseNativeMemory()
        {
            if (finalVelocity.IsCreated) finalVelocity.Dispose();
            if (currInitTouchPos.IsCreated) currInitTouchPos.Dispose();
        }
    }
}