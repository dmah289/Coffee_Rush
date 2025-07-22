using System;
using System.Collections;
using BaseSystem;
using Coffee_Rush.Board;
using Coffee_Rush.Gate;
using DG.Tweening;
using Framework.ObjectPooling;
using UnityEngine;

namespace Coffee_Rush.Block
{
    public class BLockMatcher : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D[] boxCollider2D;
        
        [Header("Matching Settings")]
        [SerializeField] private int currEmptySlotIdx;
        [SerializeField] private GateItem[] collectedGateItems;


        public bool CanSelect
        {
            get => boxCollider2D[0].enabled;
            set
            {
                for(int i = 0; i < boxCollider2D.Length; i++)
                    boxCollider2D[i].enabled = value;
            }
        }

        private void Awake()
        {
            boxCollider2D = GetComponents<BoxCollider2D>();

            CanSelect = true;
        }


        public bool MatchingAllowed {get; set;}
        
        public void AllocateGateItemsArray(int length)
        {
            collectedGateItems = new GateItem[length];
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
                    
                    if(currEmptySlotIdx == cupHolders.Length - 1)
                    {
                        CanSelect = false;
                        SelectionController.Instance.HandleMouseUp();
                    }
                    
                    GateItem item = gateController.GetMatchedItem();
                    
                    if (!item) yield break;
                    
                    collectedGateItems[currEmptySlotIdx] = item;
                    
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
            for (int i = 0; i < collectedGateItems.Length; i++)
            {
                collectedGateItems[i].JumpOnFullSlot();
                collectedGateItems[i].PackOnFullSlot();
                yield return WaitHelper.GetWait(GateItemConfig.PackingDuration * 0.3f);
            }

            yield return WaitHelper.GetWait(GateItemConfig.PackingDuration * 0.7f);
        }
        
        // TODO : Refractor feeling
        public void MoveOutOfView(eBlockType blockType)
        {
            transform.DOMoveZ(-5, BlockConfig.LiftingDuration)
                .SetEase(Ease.OutBack).OnComplete(() =>
                {
                    transform.DOScale(new Vector3(2f, 2f, 2f), 2)
                        .SetEase(Ease.OutFlash);
                    
                    float direction = transform.position.x > 0 ? 1 : -1;
                    Vector3 outOfViewPos = new Vector3(15 * direction, 0, transform.position.z);
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.transform.position = outOfViewPos;
                    transform.DOJump(outOfViewPos, 5, 1, 1f)
                        .SetEase(Ease.InFlash).OnComplete(() => PostprocessToPool(blockType));
                });
        }

        private void PostprocessToPool(eBlockType blockType)
        {
            for (int i = 0; i < collectedGateItems.Length; i++)
            {
                ObjectPooler.ReturnToPool(PoolingType.GateItem, collectedGateItems[i]);
            }
            ObjectPooler.ReturnToPool(PoolingType.BlockType00 - 1 + (byte)blockType, this);
        }
    }
}