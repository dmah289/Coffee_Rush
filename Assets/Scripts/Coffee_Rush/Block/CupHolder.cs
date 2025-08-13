using System;
using System.Collections;
using Coffee_Rush.Board;
using Coffee_Rush.Gate;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Framework.Extensions;
using UnityEngine;

namespace Coffee_Rush.Block
{
    public class CupHolder : MonoBehaviour
    {
        private static readonly int Selected = Shader.PropertyToID("_Selected");

        [Header("Self Components")]
        [SerializeField] private Transform selfTransform;
        [SerializeField] private MeshRenderer selfMeshRenderer;
        
        [Header("References")]
        [SerializeField] private Transform targetPoint;

        private void Awake()
        {
            selfTransform = transform;
            targetPoint = selfTransform.GetChild(0).GetComponent<Transform>();
            selfMeshRenderer = GetComponent<MeshRenderer>();
        }

        public void CollectGateItem(GateItem item)
        {
            item.transform.SetParent(targetPoint);

            item.transform.DOLocalJump(Vector3.zero, 5, 1, GateItemConfig.MoveDuration)
                .SetEase(Ease.OutFlash)
                .OnStart(() => item.OnJumpedToSlot(targetPoint.parent.parent));
        }

        public void Setup(eColorType colorType)
        {
            selfMeshRenderer.SetTextureOffsetByColor(colorType);
        }

        public void ShowOutline(bool selected)
        {
            // MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            // selfMeshRenderer.GetPropertyBlock(mpb);
            // mpb.SetFloat(Selected, selected ? 1 : 0);
            // selfMeshRenderer.SetPropertyBlock(mpb);

        }
    }
}