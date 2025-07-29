using System;
using System.Collections;
using System.Threading;
using BaseSystem;
using Coffee_Rush.Board;
using Coffee_Rush.Gate;
using Cysharp.Threading.Tasks;
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
        [SerializeField] private bool isBusy;

        public static event Action OnBlockFullSlot;

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
            boxCollider2D = GetComponentsInChildren<BoxCollider2D>();

            CanSelect = true;
        }


        public bool IsMatching {get; set;}
        
        public void AllocateGateItemsArray(int length)
        {
            collectedGateItems = new GateItem[length];
            currEmptySlotIdx = 0;
        }

        public async UniTask TryCollectGateItem(GateController gate, eBlockType blockType, eColorType colorType, CupHolder[] cupHolders, CancellationTokenSource cts)
        {
            try
            {
                // Cancel if move block out of range, stop waiting for matching
                await UniTask.WaitUntil(this, t => !t.IsMatching,cancellationToken: cts.Token);
                
                IsMatching = true;

                if (currEmptySlotIdx < cupHolders.Length)
                {
                    while (currEmptySlotIdx < cupHolders.Length && gate.ColorType == colorType)
                    {
                        // If it is matching and player move block out of range, cancel matching.
                        if (cts.IsCancellationRequested)
                        {
                            // Debug.Log("Matching cancelled: " + gate.transform.position);
                            IsMatching = false;
                            return;
                        }
                        // if (!IsMatching) break;

                        GateItem item = gate.GetCollectableItem();

                        collectedGateItems[currEmptySlotIdx] = item;
                        await cupHolders[currEmptySlotIdx++].CollectGateItem(item);

                        if (currEmptySlotIdx == cupHolders.Length)
                        {
                            OnBlockFullSlot?.Invoke();
                            CanSelect = false;
                            SelectionController.Instance.HandleMouseUp();
                            await PackAllGateItems();
                            MoveOutOfView(blockType);
                        }

                        await UniTask.Yield();
                    }
                }

                IsMatching = false;

                await UniTask.Yield();
            }
            catch (OperationCanceledException)
            {
                IsMatching = false;
                // Debug.Log("Cancel waiting for matching : " + gate.transform.position);
            }
        }

        private async UniTask PackAllGateItems()
        {
            for (int i = 0; i < collectedGateItems.Length; i++)
            {
                collectedGateItems[i].JumpOnFullSlot();
                collectedGateItems[i].PackOnFullSlot();
                await UniTask.Delay((int)(GateItemConfig.PackingDuration * 1000 * 0.3f));
            }

            await UniTask.Delay((int)(GateItemConfig.PackingDuration * 1000 * 0.7f));
        }
        
        public void MoveOutOfView(eBlockType blockType)
        {
            transform.DOMoveZ(-5, BlockConfig.LiftingDuration)
                .SetEase(Ease.OutBack).OnComplete(() =>
                {
                    transform.DOScale(BlockConfig.targetScaleToMove, BlockConfig.LiftingDuration)
                        .SetEase(Ease.OutFlash);
                    
                    float direction = transform.position.x > 0 ? 1 : -1;
                    Vector3 outOfViewPos = new Vector3(
                        (BoardLayoutGenerator.Instance.HalfWidthWorldPos + 10) * direction,
                        BoardLayoutGenerator.Instance.HalfHeightWorldPos + 5,
                        0);
                    transform.DOJump(outOfViewPos, 5, 1, BlockConfig.LiftingDuration)
                        .SetEase(Ease.InFlash).OnComplete(() =>
                        {
                            PostprocessToPool(blockType);
                            BoardController.Instance.DecreaseBlockCount();
                        });
                });
        }

        public void PostprocessToPool(eBlockType blockType)
        {
            for (int i = 0; i < currEmptySlotIdx; i++)
            {
                ObjectPooler.ReturnToPool(PoolingType.GateItem, collectedGateItems[i]);
            }
            ObjectPooler.ReturnToPool(PoolingType.BlockType00 - 1 + (byte)blockType, this);
        }
    }
}