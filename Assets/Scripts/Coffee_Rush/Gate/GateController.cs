using System;
using System.Collections.Generic;
using Coffee_Rush.Gate;
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
        
        
        [Header("GateItem Manager")]
        [SerializeField] private List<GateItem> gateItems;
        [SerializeField] private eDirection gateDir;
        

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
            
            SpawnGateItems(gateDir, itemColors);
        }
        
        private void SpawnGateItems(eDirection gateDir, eColorType[] itemColors)
        {
            gateItems.Clear();
            ColorType = itemColors[0];
            
            for (int i = 0; i < itemColors.Length; i++)
            {
                GateItem gateItem = ObjectPooler.GetFromPool<GateItem>(PoolingType.GateItem, selfTransform);
                
                gateItem.transform.localPosition = new Vector3(
                    GateItemConfig.firstItemSpawnedPosByDir[(byte)gateDir - 1].x,
                    GateItemConfig.firstItemSpawnedPosByDir[(byte)gateDir - 1].y - i * GateItemConfig.Distance,
                    GateItemConfig.firstItemSpawnedPosByDir[(byte)gateDir - 1].z);
                gateItem.transform.localEulerAngles = GateItemConfig.RotationsByDir[(byte)gateDir - 1];
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
                    ShiftBehindElementsAfterRemove();
                }
                
                return item;
            }

            return null;
        }

        private void ShiftBehindElementsAfterRemove()
        {
            for (int i = 0; i < gateItems.Count; i++)
            {
                gateItems[i].transform.DOKill();

                Vector3 targetPos = new Vector3(
                    GateItemConfig.firstItemSpawnedPosByDir[(byte)gateDir - 1].x,
                    GateItemConfig.firstItemSpawnedPosByDir[(byte)gateDir - 1].y - i * GateItemConfig.Distance,
                    GateItemConfig.firstItemSpawnedPosByDir[(byte)gateDir - 1].z);
                
                gateItems[i].transform.DOLocalMove(targetPos, GateItemConfig.MoveDuration)
                    .SetEase(Ease.OutFlash);
            }
        }
    }
}