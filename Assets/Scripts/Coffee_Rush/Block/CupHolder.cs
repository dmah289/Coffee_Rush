using System;
using System.Collections;
using Coffee_Rush.Board;
using Coffee_Rush.Gate;
using DG.Tweening;
using Framework.Extensions;
using UnityEngine;

namespace Coffee_Rush.Block
{
    public class CupHolder : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private Transform selfTransform;
        [SerializeField] private MeshRenderer selfMeshRenderer;
        private MaterialPropertyBlock mpb;
        
        [Header("References")]
        [SerializeField] private Transform targetPoint;

        private void Awake()
        {
            selfTransform = transform;
            targetPoint = selfTransform.GetChild(0).GetComponent<Transform>();
            selfMeshRenderer = GetComponent<MeshRenderer>();
            
            mpb = new MaterialPropertyBlock();
        }


        public IEnumerator AttractGateItem(GateItem item)
        {
            item.transform.SetParent(targetPoint, true);
            
            item.OnJumpedToSlot();
            
            Tween jumpTween = item.transform.DOLocalJump(Vector3.zero, 5, 1,GateItemConfig.MoveDuration)
                .SetEase(Ease.OutFlash);
            yield return jumpTween.WaitForCompletion();

        }

        public void Setup(eColorType colorType)
        {
            selfMeshRenderer.SetTextureOffsetByColor(colorType);
        }
    }
}