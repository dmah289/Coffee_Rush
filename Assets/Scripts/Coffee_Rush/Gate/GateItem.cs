﻿using System;
using Coffee_Rush.Gate;
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
            
            transform.position = initPos;
            transform.eulerAngles = GateItemConfig.WorldRotation;
            ColorType = colorType;
        }

        public void OnJumpedToSlot(Transform parent)
        {
            selfTransform.rotation = Quaternion.LookRotation(parent.forward, parent.up);
        }

        public void PackOnFullSlot()
        {
            cupLidTransform.localScale = Vector3.one;
            cupLidTransform.localPosition = GateItemConfig.CupLidFloatingPos;

            cupLidTransform.DOLocalMoveY(0, GateItemConfig.PackingDuration);
        }

        public void JumpOnFullSlot()
        {
            selfTransform.DOLocalMoveY(1, GateItemConfig.PackingDuration / 2)
                .OnComplete(() => selfTransform.DOLocalMoveY(0, GateItemConfig.PackingDuration / 2));
        }
    }
}