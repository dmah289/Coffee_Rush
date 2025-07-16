using System;
using Coffee_Rush.Board;
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
        [SerializeField] private Transform target;

        private void Awake()
        {
            selfTransform = transform;
            target = selfTransform.GetChild(0).GetComponent<Transform>();
            selfMeshRenderer = GetComponent<MeshRenderer>();
            
            mpb = new MaterialPropertyBlock();
        }


        public void AttractGateItem(GateItem item)
        {
            item.transform.SetParent(target);
            item.transform.localRotation = Quaternion.identity;
            item.transform.DOLocalMove(Vector3.zero, 0.2f).SetEase(Ease.OutFlash);
        }

        public void Setup(eColorType colorType)
        {
            selfMeshRenderer.SetTextureOffsetByColor(colorType);
        }
    }
}