using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Coffee_Rush.JobCalculation
{
    [BurstCompile]
    public struct BalancingJob : IJob
    {
        // Configuration
        [ReadOnly] public float3 InitialEuler;
        [ReadOnly] public float DampingFactor;
        [ReadOnly] public float TiltSensitivity;
        [ReadOnly] public float MaxOffset;
        
        // Input
        [ReadOnly] public float3 CenterToInitTouchOffset;
        [ReadOnly] public float3 CurTouchPos;
        [ReadOnly] public float3 CurBlockPos;
        [ReadOnly] public bool IsDragging;
        [ReadOnly] public float DeltaTime;
        
        // Output
        public NativeReference<float3> CurrentEuler;
        
        public void Execute()
        {
            float3 currInitTouchPos = CurBlockPos + CenterToInitTouchOffset;
            float3 targetEuler = InitialEuler;

            if (IsDragging)
            {
                float eulerOffsetY = (currInitTouchPos.x - CurTouchPos.x) * TiltSensitivity;
                float eulerOffsetX = (CurTouchPos.y - currInitTouchPos.y) * TiltSensitivity;

                targetEuler.x += eulerOffsetX;
                targetEuler.y += eulerOffsetY;
                
                targetEuler.x = math.clamp(targetEuler.x, InitialEuler.x - MaxOffset, InitialEuler.x + MaxOffset);
                targetEuler.y = math.clamp(targetEuler.y, InitialEuler.y - MaxOffset, InitialEuler.y + MaxOffset);
            }
            
            CurrentEuler.Value = math.lerp(CurrentEuler.Value, targetEuler, DampingFactor * DeltaTime);
        }
    }
}