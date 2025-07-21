using System;
using System.Collections;
using Coffee_Rush.Board;
using Coffee_Rush.Gate;
using DG.Tweening;
using Framework.ObjectPooling;
using UnityEngine;

namespace Coffee_Rush.Block
{
    public class BLockMatcher : MonoBehaviour
    {
        [Header("Matching Settings")]
        [SerializeField] private int currEmptySlotIdx;
        [SerializeField] private GateItem[] currGateItems;
        
        
        public bool MatchingAllowed {get; set;}
        
        public void AllocateGateItemsArray(int length)
        {
            currGateItems = new GateItem[length];
            currEmptySlotIdx = 0;
        }


        public IEnumerator TryCollectGateItem(Collider2D other, eBlockType blockType, eColorType colorType, CupHolder[] cupHolders)
        {
            MatchingAllowed = true;
            
            if (other.gameObject.TryGetComponent(out GateController gateController) && currEmptySlotIdx < cupHolders.Length)
            {
                yield return null;
                
                while (currEmptySlotIdx < cupHolders.Length && gateController.ColorType == colorType)
                {
                    if (!MatchingAllowed) yield break;
                    
                    GateItem item = gateController.GetMatchedItem();
                    currGateItems[currEmptySlotIdx] = item;

                    if (!item) yield break;
                    
                    yield return cupHolders[currEmptySlotIdx++].AttractGateItem(item);

                    if (currEmptySlotIdx == cupHolders.Length)
                    {
                        yield return PackAllGateItems();
                        MoveOutOfView(blockType);
                    }

                    yield return null;
                }
            }
        }

        private IEnumerator PackAllGateItems()
        {
            for (int i = 0; i < currGateItems.Length; i++)
            {
                currGateItems[i].PackOnFullSlot();
                yield return WaitHelper.GetWait(GateItemConfig.PackingDuration * 0.3f);
            }

            yield return WaitHelper.GetWait(GateItemConfig.PackingDuration * 0.7f);
        }
        
        public void MoveOutOfView(eBlockType blockType)
        {
            Sequence sequence = DOTween.Sequence();
            
            sequence.Append(transform.DOMoveZ(-12, BlockConfig.LiftingDuration)
                .SetEase(Ease.OutBack));
            float direction = transform.position.x > 0 ? 1 : -1;
            sequence.Append(transform.DOMoveX(direction * 20, BlockConfig.LiftingDuration)
                .SetEase(Ease.InOutBack));

            sequence.OnComplete(() => PostprocessToPool(blockType));
        }

        private void PostprocessToPool(eBlockType blockType)
        {
            for (int i = 0; i < currGateItems.Length; i++)
            {
                ObjectPooler.ReturnToPool(PoolingType.GateItem, currGateItems[i]);
            }
            ObjectPooler.ReturnToPool(PoolingType.BlockType00 - 1 + (byte)blockType, this);
        }
    }
}