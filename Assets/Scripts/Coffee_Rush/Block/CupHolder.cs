using System;
using Coffee_Rush.Board;
using DG.Tweening;
using UnityEngine;

namespace Coffee_Rush.Block
{
    public class CupHolder : MonoBehaviour
    {
        [SerializeField] private Transform selfTransform;
        [SerializeField] private Transform target;

        private void Awake()
        {
            target = transform.GetChild(0).GetComponent<Transform>();
            selfTransform = transform;
        }


        public void AttractGateItem(GateItem item)
        {
            item.transform.SetParent(target);
            
            item.transform.DOLocalMove(Vector3.zero, 0.2f).SetEase(Ease.OutFlash);
        }
    }
}