using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Coffee_Rush.JobCalculation
{
    public struct VelocityCalculationJob : IJob
    {
        [ReadOnly] public float3 CenterToTouchOffset;
        [ReadOnly] public float3 CurrentTouchPos;
        [ReadOnly] public float3 CurrentBlockPos;
        [ReadOnly] public float Speed;

        public NativeReference<float3> Velocity;
        
        public void Execute()
        {
            float3 CurrentLocalTouchPoint = CurrentBlockPos + CenterToTouchOffset;

            float3 CurrentTouchToLocalTouchDir = CurrentTouchPos - CurrentLocalTouchPoint;
            float distanceSqr = math.lengthsq(CurrentTouchToLocalTouchDir);

            if (distanceSqr > 1e-5f)
            {
                CurrentTouchToLocalTouchDir = math.normalize(CurrentTouchToLocalTouchDir);
                float distance = math.sqrt(distanceSqr);
                if (distance > 3) distance = 3;
                else distance *= distance / 2f;
                
                Velocity.Value = distance * Speed * CurrentTouchToLocalTouchDir;
            }
            else Velocity.Value = float3.zero;
        }
    }
}