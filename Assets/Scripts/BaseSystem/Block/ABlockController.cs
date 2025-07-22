using Coffee_Rush.Block;
using Coffee_Rush.Board;
using Coffee_Rush.JobCalculation;
using DG.Tweening;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace BaseSystem.Block
{
    public abstract class ABlockController : MonoBehaviour, ISelectable
    {
        [Header("Self Components")]
        [SerializeField] protected Transform selfTransform;
        [SerializeField] protected Rigidbody2D selfRb;
        
        [Header("Child Components")]
        [SerializeField] protected CupHolder[] cupHolders;
        
        [Header("Movement Data")]
        [SerializeField] protected Vector3 centerToTouchOffset;
        [SerializeField] protected bool isDragging;
        
        [Header("Data")]
        [SerializeField] protected eColorType colorType;
        [SerializeField] protected eBlockType blockType;
        
        //Velocity Calculation Job
        protected VelocityCalculationJob velocityCalculationJob;
        protected JobHandle velocityCalculationJobHandle;
        protected NativeReference<float3> finalVelocity;
        protected NativeReference<float3> currInitTouchPos;
        
        
        public eBlockType BlockType
        {
            get => blockType;
            set
            {
                if (blockType != value)
                {
                    blockType = value;
                }
            }
        }
        
        public eColorType ColorType
        {
            get => colorType;
            set
            {
                if (colorType != value)
                {
                    colorType = value;
                    ChangeColors(cupHolders);
                }
            }
        }

        protected virtual void Awake()
        {
            selfTransform = transform;
            selfRb = selfTransform.GetComponent<Rigidbody2D>();
            
            cupHolders = GetComponentsInChildren<CupHolder>();

            selfRb.isKinematic = true;
        }

        protected virtual void OnEnable()
        {
            InitializeAllJobs();
        }

        private void Start()
        {
            ColorType = colorType;
        }

        public void ChangeColors(CupHolder[] cupHolders)
        {
            for (int i = 0; i < cupHolders.Length; i++)
            {
                cupHolders[i].Setup(colorType);
            }
        }

        protected virtual void InitializeAllJobs()
        {
            finalVelocity = new(Allocator.Persistent);
            currInitTouchPos = new (Allocator.Persistent);
            velocityCalculationJob = new VelocityCalculationJob
            {
                Speed = BlockConfig.Speed,
                CurInitTouchPos = currInitTouchPos,
                Velocity = finalVelocity
            };
        }

        public virtual void OnSelect(Vector3 mousePos)
        {
            DOTween.Kill(gameObject);
            transform.DOMoveZ(-1f, 0.2f);
            selfRb.isKinematic = false;
            
            SetupJobConfigs(mousePos);
        }

        protected void SetupJobConfigs(Vector3 mousePos)
        {
            centerToTouchOffset = mousePos - selfTransform.position;
            centerToTouchOffset.z = 0;
            velocityCalculationJob.CenterToInitTouchOffset = centerToTouchOffset;
        }

        public void OnDrag(Vector3 currTouchPos)
        {
            if (gameObject.activeSelf)
            {
                isDragging = true;
            
                ScheduleAllJobs(currTouchPos);
            
                JobHandle.ScheduleBatchedJobs();
            }
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

        public virtual void OnDeselect()
        {
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