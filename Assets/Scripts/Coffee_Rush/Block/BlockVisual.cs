using System;
using Unity.Mathematics;
using UnityEngine;

namespace Coffee_Rush.Block
{
    public class BlockVisual : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private Transform selfTransform;

        private void Awake()
        {
            selfTransform = transform;
            selfTransform.localEulerAngles = BlockConfig.initEulerModel;
        }


        public Vector3 EulerRotation
        {
            get => selfTransform.localEulerAngles;
            set => selfTransform.localEulerAngles = value;
        }
    }
}