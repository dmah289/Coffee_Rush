using System;
using BaseSystem;
using Coffee_Rush.Board;
using Coffee_Rush.Level;
using DG.Tweening;
using UnityEngine;

namespace Coffee_Rush.Block
{
    public class BlockFitting : ABlockFitting
    {
        public void FitBoard()
        {
            // Get world position of check point
            Vector3 worldPos = checkPoint.position;
            
            Vector3 coordPos = BoardLayoutGenerator.Instance.GetCoordPos(worldPos);

            Vector3 target = coordPos + centerToCheckPointOffset;
            transform.DOMove(target, 0.2f).SetEase(Ease.OutFlash);
        }
        
        public void SetCheckPointToTargetTile(Vector3 targetTilePos)
        {
            Vector3 target = targetTilePos + centerToCheckPointOffset;
            transform.position = target;
        }
    }
}