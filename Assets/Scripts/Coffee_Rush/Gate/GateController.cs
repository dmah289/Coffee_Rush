using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using Coffee_Rush.Block;
using Coffee_Rush.Gate;
using Coffee_Rush.Level;
using DG.Tweening;
using Framework.Extensions;
using Framework.ObjectPooling;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Coffee_Rush.Board
{
    public class GateController : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private Transform selfTransform;
        [SerializeField] private MeshRenderer selfMeshRenderer;
        [SerializeField] private Transform checkPoint;
        private Collider2D[] colliders = new Collider2D[1];
        [SerializeField] private BlockController currBlock;
        private CancellationTokenSource cts;
        
        
        [Header("GateItem Manager")]
        [SerializeField] private List<GateItem> gateItems;
        [SerializeField] private eDirection gateDir;
        [SerializeField] private eColorType colorType;


        public eDirection GateDir => gateDir;
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

        public void Setup(Vector3 tilePos,eDirection gateDir, eColorType[] itemColors, CompressedItemPath itemsPath)
        {
            this.gateDir = gateDir;
            
            Vector3 pos = new Vector3(
                tilePos.x + GateConfig.GateFitTileDir[(byte)gateDir - 1].x * BoardConfig.cellSize / 2,
                tilePos.y + GateConfig.GateFitTileDir[(byte)gateDir - 1].y * BoardConfig.cellSize / 2,
                tilePos.z);
            
            if(gateDir == eDirection.Up) pos.y += GateConfig.GateWidth / 2;
            else if(gateDir == eDirection.Down) pos.y -= GateConfig.GateWidth / 2;
            else if(gateDir == eDirection.Left) pos.x -= GateConfig.GateWidth / 2;
            else if(gateDir == eDirection.Right) pos.x += GateConfig.GateWidth / 2;
            
            selfTransform.position = pos;
            selfTransform.eulerAngles = new Vector3(0, 0, GateConfig.GateZRotByDir[(byte)gateDir-1]);
            
            SpawnGateItems(gateDir, itemColors, itemsPath);
        }

        public void SpawnGateItems(eDirection gateDir, eColorType[] itemColors, CompressedItemPath itemsPath)
        {
            gateItems.Clear();
            
            ColorType = itemColors[0];

            Vector3 curPos = selfTransform.position + GateItemConfig.FirstItemToGateOffset[(byte)gateDir - 1];
            
            int currTurnIdx = 0;

            for (int i = 0; i < itemColors.Length; i++)
            {
                GateItem gateItem = ObjectPooler.GetFromPool<GateItem>(PoolingType.GateItem);

                if (currTurnIdx < itemsPath.turnIndices.Length - 1 && i >= itemsPath.turnIndices[currTurnIdx + 1])
                    currTurnIdx++;

                if (i != 0)
                    curPos += GateItemConfig.ItemDir[(byte)itemsPath.turnDirections[currTurnIdx] - 1] * GateItemConfig.Distance;
                
                gateItem.transform.position = curPos;
                gateItem.transform.eulerAngles = GateItemConfig.WorldRotation;
                gateItem.ColorType = itemColors[i];
                
                gateItems.Add(gateItem);
            }
        }

        private void Update()
        {
            FindMatchableBlock();
        }

        private void FindMatchableBlock()
        {
            int numHits = Physics2D.OverlapPointNonAlloc(checkPoint.position, colliders);
            if (numHits > 0)
            {
                if (!currBlock)
                {
                    currBlock = colliders[0].GetComponent<BlockController>();
                    if (currBlock)
                    {
                        cts = new CancellationTokenSource();
                        currBlock.TryCollectGateItems(this, cts);
                    }
                }
            }
            else
            {
                if (currBlock)
                {
                    if (cts != null)
                    {
                        cts.Cancel();
                        cts.Dispose();
                        cts = null;
                    }
                    
                    currBlock.DisableMatching();
                    currBlock = null;
                }
            }
        }

        public GateItem GetCollectableItem()
        {
            if (gateItems.Count == 0)
                return null;
            
            GateItem item = gateItems[0];

            selfTransform.DOMove(selfTransform.position + GateConfig.impulseOffset, GateItemConfig.MoveDuration / 2)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => selfTransform.position -= GateConfig.impulseOffset);
            
            UpdateGateColor();

            if (gateItems.Count > 1)
                ShiftRemainingElementsBeforeRemove();
            
            gateItems.RemoveAt(0);
                
            return item;
        }

        private void UpdateGateColor()
        {
            ColorType = gateItems.Count == 1 ? eColorType.None : gateItems[1].ColorType;
        }

        private void ShiftRemainingElementsBeforeRemove()
        {
            for (int i = 1; i < gateItems.Count; i++)
            {
                Vector3 targetPos = gateItems[i-1].transform.position;
                
                gateItems[i].transform.DOLocalMove(targetPos, GateItemConfig.MoveDuration)
                    .SetEase(Ease.OutFlash);
            }
        }

        public void OnRevokenToPool()
        {
            for(int i = 0; i < gateItems.Count; i++)
            {
                ObjectPooler.ReturnToPool(PoolingType.GateItem, gateItems[i]);
            }
            gateItems.Clear();
            
            ObjectPooler.ReturnToPool(PoolingType.Gate, this);
        }
    }
}