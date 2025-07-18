using System;
using System.Collections.Generic;
using DG.Tweening;
using Framework.Extensions;
using Framework.ObjectPooling;
using UnityEngine;

namespace Coffee_Rush.Board
{
    public class GateController : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private Transform selfTransform;
        [SerializeField] private MeshRenderer selfMeshRenderer;
        private MaterialPropertyBlock mpb;
        [SerializeField] private int[] gateZRotByDir = new int[4]{ 0, -90, -180, -90 };

        [Header("GateItem Config")]
        [SerializeField] private float gateItemDistance = 0.8f;
        [SerializeField] private float cellSize = 2f;
        [SerializeField] private Vector3[] gateItemRotationsByDir = new  Vector3[4]
        {
            new Vector3(-75, 0, 0),      // Up
            new Vector3(-90, 0, 30),    // Right
            new Vector3(-110, 0, 0),   // Down
            new Vector3(-90, 0, -30)     // Left
        };
        [SerializeField] private Vector2Int[] gateOffsetByDir = new Vector2Int[4]
        {
            new (0, 1),      // Up
            new (1, 0),      // Right
            new (0, -1),     // Down
            new (0, -1)       // Left
        };
        
        [Header("GateItem Manager")]
        [SerializeField] private List<GateItem> gateItems;
        

        private eColorType colorType;
        public eColorType ColorType
        {
            get => colorType;
            set
            {
                if (colorType != value)
                {
                    colorType = value;
                    selfMeshRenderer.SetTextureOffsetByColor(colorType);
                }
            }
        }

        private void Awake()
        {
            selfTransform = transform;
        }

        public void Setup(Vector3 tilePos,eDirection gateDir, eColorType[] itemColors)
        {
            selfTransform.position = new Vector3(
                tilePos.x + gateOffsetByDir[(byte)gateDir].x * cellSize / 2, tilePos.y + gateOffsetByDir[(byte)gateDir].y * cellSize / 2, tilePos.z);
            selfTransform.eulerAngles = new Vector3(0, 0, gateZRotByDir[(byte)gateDir-1]);
            SpawnGateItems(gateDir, itemColors);
        }
        
        private void SpawnGateItems(eDirection gateDir, eColorType[] itemColors)
        {
            float spawnPosZ = gateDir == eDirection.Down ? -1 : 0;
            gateItems.Clear();
            ColorType = itemColors[0];
            for (int i = 0; i < itemColors.Length; i++)
            {
                GateItem gateItem = ObjectPooler.GetFromPool<GateItem>(PoolingType.Cup, selfTransform);
                gateItem.transform.localPosition = new Vector3(1, 1 + i * gateItemDistance, spawnPosZ);
                gateItem.transform.localEulerAngles = gateItemRotationsByDir[(byte)gateDir - 1];
                gateItem.ColorType = itemColors[i];
                
                gateItems.Add(gateItem);
            }
        }

        public GateItem GetMatchedItem()
        {
            if (gateItems.Count > 0)
            {
                GateItem item = gateItems[0];
                gateItems.RemoveAt(0);
                
                if(gateItems.Count == 0) ColorType = eColorType.None;
                else
                {
                    ColorType = gateItems[0].ColorType;
                    ShiftBehindElements();
                }
                
                return item;
            }

            return null;
        }

        private void ShiftBehindElements()
        {
            for (int i = 0; i < gateItems.Count; i++)
            {
                gateItems[i].transform.DOKill();
                gateItems[i].transform.DOLocalMove(
                        new Vector3(1, 1 + i * gateItemDistance, gateItems[i].transform.localPosition.z), 
                        0.3f)
                    .SetEase(Ease.OutFlash);
            }
        }
    }
}