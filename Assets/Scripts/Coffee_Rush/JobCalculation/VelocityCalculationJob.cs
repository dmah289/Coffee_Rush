using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Coffee_Rush.JobCalculation
{
    [BurstCompile]
    public struct VelocityCalculationJob : IJob
    {
        [ReadOnly] public float3 CenterToInitTouchOffset;
        [ReadOnly] public float3 CurTouchPos;
        [ReadOnly] public float3 CurBlockPos;
        [ReadOnly] public float Speed;

        public NativeReference<float3> CurInitTouchPos;
        public NativeReference<float3> Velocity;
        
        public void Execute()
        {
            CurInitTouchPos.Value = CurBlockPos + CenterToInitTouchOffset;

            float3 curTouchToInitTouchDir = CurTouchPos - CurInitTouchPos.Value;
            float distanceSqr = math.lengthsq(curTouchToInitTouchDir);

            if (distanceSqr > 1e-5f)
            {
                curTouchToInitTouchDir = math.normalize(curTouchToInitTouchDir);
                float distance = math.sqrt(distanceSqr);
                if (distance > 3) distance = 3;
                else distance *= distance / 2f;
                
                Velocity.Value = distance * Speed * curTouchToInitTouchDir;
            }
            else Velocity.Value = float3.zero;
        }
    }
}