using System;
using Coffee_Rush.Level;
using DG.Tweening;
using UnityEngine;

namespace Coffee_Rush.Block
{
    [RequireComponent(typeof(BlockController))]
    public class BlockFitting : MonoBehaviour
    {
        [SerializeField] private Transform checkPoint;
        
        [SerializeField] private Vector3 centerToCheckPointOffset;

        public void FitBoard()
        {
            // Get world position of check point
            Vector3 worldPos = checkPoint.position;
            Vector3 coordPos = LevelManager.Instance.boardController.layoutGenerator.GetCoordPos(worldPos);

            Vector3 target = coordPos + centerToCheckPointOffset;
            transform.DOMove(target, 0.2f).SetEase(Ease.OutFlash);
        }

        public void CalculateCheckPointOffset()
        {
            centerToCheckPointOffset = transform.position - checkPoint.position;
        }
    }
}