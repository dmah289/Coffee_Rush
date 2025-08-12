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
        [SerializeField] private BlockController blockController;
        [SerializeField] private BlockVisual blockVisual;
        
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
            blockController = GetComponent<BlockController>();
            blockVisual = GetComponentInChildren<BlockVisual>();

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
                        cupHolders[currEmptySlotIdx++].CollectGateItem(item);
                        await UniTask.Delay((int)(GateItemConfig.MoveDuration * 0.15f * 1000));
                        

                        if (currEmptySlotIdx == cupHolders.Length)
                        {
                            OnBlockFullSlot?.Invoke();
                            BoardController.Instance.BlockCount--;
                            CanSelect = false;
                            SelectionController.Instance.DeselectCurrentObject(blockController);
                            
                            await UniTask.Delay((int)(GateItemConfig.MoveDuration * 0.85f * 1000));
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
            await transform.DOMoveZ(-7, 0.3f)
                .SetEase(Ease.OutBack)
                .AsyncWaitForCompletion();
            
            for (int i = 0; i < collectedGateItems.Length; i++)
            {
                collectedGateItems[i].JumpOnFullSlot();
                collectedGateItems[i].PackOnFullSlot();
                await UniTask.Delay((int)(GateItemConfig.PackingDuration * 1000 / collectedGateItems.Length));
            }
            await UniTask.Delay((int)(GateItemConfig.PackingDuration * 1000 * 0.5f * (1 - 1f / collectedGateItems.Length)));
        }
        
        public void MoveOutOfView(eBlockType blockType)
        {
            float direction = transform.position.x > 0 ? 1 : -1;
            Vector3 outOfViewPos = new Vector3(
                (BoardLayoutGenerator.Instance.HalfWidthWorldPos + 10) * direction,
                BoardLayoutGenerator.Instance.HalfHeightWorldPos,
                -15);

            float moveDuration = Vector3.Distance(transform.position, outOfViewPos) / BlockConfig.SpeedToMoveOutOfView;
            
            blockVisual.TiltOnMoveOutOfView(-direction, moveDuration);
            transform.DOScale(BlockConfig.TargetScaleToMove, moveDuration);
            transform.DOMove(outOfViewPos, moveDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    ReturnGateItemsToPool();
                    ObjectPooler.ReturnToPool(PoolingType.BlockType00 - 1 + (byte)blockType, blockController);
                });
        }

        public void ReturnGateItemsToPool()
        {
            for (int i = 0; i < currEmptySlotIdx; i++)
            {
                ObjectPooler.ReturnToPool(PoolingType.GateItem, collectedGateItems[i]);
                collectedGateItems[i] = null;
            }

            currEmptySlotIdx = 0;
        }
    }
}