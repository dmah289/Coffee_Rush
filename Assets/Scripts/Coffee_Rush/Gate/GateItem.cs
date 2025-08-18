using System;
using Coffee_Rush.Gate;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Framework.Extensions;
using UnityEngine;

namespace Coffee_Rush.Board
{
    public class GateItem : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private MeshRenderer visualMeshRenderer;
        [SerializeField] private MeshRenderer cupLidMeshRenderer;
        [SerializeField] private Transform selfTransform;
        [SerializeField] private Transform cupLidTransform;

        public float elapsedTime;
        public bool isBalancing;
        float balancingDuration = 0.07f;
        

        private eColorType colorType;
        public eColorType ColorType
        {
            get => colorType;
            set
            {
                if(colorType != value)
                {
                    colorType = value;
                    cupLidMeshRenderer.SetTextureOffsetByColor(colorType);
                    visualMeshRenderer.SetTextureOffsetByColor(colorType);
                }
            }
        }

        public void SetupOnLevelStarted(Vector3 initPos, eColorType colorType)
        {
            cupLidTransform.localScale = Vector3.zero;
            selfTransform.localScale = Vector3.one;
            
            transform.position = initPos;
            transform.eulerAngles = GateItemConfig.WorldRotation;
            ColorType = colorType;
        }

        public void PackOnFullSlot()
        {
            cupLidTransform.localScale = Vector3.one;
            cupLidTransform.localPosition = GateItemConfig.CupLidFloatingPos;

            cupLidTransform.DOLocalMoveY(0, GateItemConfig.PackingDuration);
        }

        public void JumpOnFullSlot()
        {
            selfTransform.DOLocalMoveY(2, GateItemConfig.PackingDuration / 2)
                .OnComplete(() => selfTransform.DOLocalMoveY(0, GateItemConfig.PackingDuration / 2));
        }

        // TODO : Consider using Job System for learning purposes
        public async UniTaskVoid BalanceOnBlock(Quaternion targetRotation)
        {
            if (!isBalancing)
            {
                isBalancing = true;

                Quaternion initRotation = selfTransform.rotation;
                float startTime = Time.time;
                while (Time.time - startTime < balancingDuration)
                {
                    float elapsedTime = Time.time - startTime;
                    float normalizedTime = elapsedTime / balancingDuration;
                    if (selfTransform != null)
                        selfTransform.rotation = Quaternion.Slerp(initRotation, targetRotation, normalizedTime);
                    await UniTask.Yield();
                }
                if (selfTransform != null)
                    selfTransform.rotation = targetRotation;
                
                isBalancing = false;
            }
        }
    }
}