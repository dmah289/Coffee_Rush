using Coffee_Rush.Block;
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
        [SerializeField] private Transform selfTransform;
        [SerializeField] private Rigidbody2D selfRb;
        
        [Header("Attributes")]
        [SerializeField] private eBlockType blockType;
        
        [Header("Movement Settings")]
        [SerializeField] private Vector3 centerToTouchOffset;
        [SerializeField] private float speed;
        [SerializeField] private bool isDragging;
        
        [Header("Velocity Calculation Job")]
        private VelocityCalculationJob velocityCalculationJob;
        private JobHandle velocityCalculationJobHandle;
        private NativeReference<float3> finalVelocity;
        

        private void Awake()
        {
            selfTransform = transform;
            selfRb = selfTransform.GetComponent<Rigidbody2D>();

            selfRb.isKinematic = true;
            speed = 30;

            finalVelocity = new(Allocator.Persistent);
            velocityCalculationJob = new VelocityCalculationJob
            {
                Speed = speed,
                Velocity = finalVelocity
            };
        }

        public void OnSelect(Vector3 mousePos)
        {
            transform.DOMoveZ(-1f, 0.2f);
            centerToTouchOffset = mousePos - selfTransform.position;
            centerToTouchOffset.z = 0;
            
            selfRb.isKinematic = false;
            velocityCalculationJob.CenterToTouchOffset = centerToTouchOffset;
        }

        public void OnDrag(Vector3 currTouchPos)
        {
            isDragging = true;
            
            velocityCalculationJob.CurrentTouchPos = currTouchPos;
            Vector3 currBlockPos = selfTransform.position;
            currBlockPos.z = 0f;
            velocityCalculationJob.CurrentBlockPos = currBlockPos;

            velocityCalculationJobHandle = velocityCalculationJob.Schedule();
            JobHandle.ScheduleBatchedJobs();
        }

        private void LateUpdate()
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

        private void OnDestroy()
        {
            if (finalVelocity.IsCreated) finalVelocity.Dispose();
        }
    }
}