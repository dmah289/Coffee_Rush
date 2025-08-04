using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Framework.Extensions
{
    public static class RectTransformExtensions
    {
        public static async UniTask LerpToTarget(this RectTransform rectTransform, Vector2 targetPos, float duration, CancellationToken token)
        {
            Vector2 startPos = rectTransform.anchoredPosition;
            float elapsedTime = 0f;

            try
            {
                while (elapsedTime < duration)
                {
                    // if (token.IsCancellationRequested)
                    //     return;
                    token.ThrowIfCancellationRequested();
                    
                    elapsedTime += Time.deltaTime;
                    float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsedTime / duration));
                    rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
                    
                    await UniTask.Yield(token);
                }
                
                rectTransform.anchoredPosition = targetPos;
            }
            catch (OperationCanceledException) {}

        }

        public static async UniTaskVoid MoveToTargetBySpeed(this RectTransform rectTransform, Vector2 targetPos, float speed,
            CancellationToken token)
        {
            Vector2 startPos = rectTransform.anchoredPosition;
            Vector2 direction = (targetPos - startPos).normalized;
            
            try
            {
                while (Vector2.Distance(rectTransform.anchoredPosition, targetPos) > 1f)
                {
                    token.ThrowIfCancellationRequested();
                    
                    Vector2 movement = direction * speed * Time.deltaTime;

                    if (Vector2.Distance(rectTransform.anchoredPosition, targetPos) < movement.magnitude)
                        break;
                    
                    rectTransform.anchoredPosition += movement;
                    
                    await UniTask.Yield(token);
                }
                rectTransform.anchoredPosition = targetPos;
            }
            catch (OperationCanceledException) {}
        }
    }
}