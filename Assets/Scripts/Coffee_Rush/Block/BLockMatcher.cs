using System;
using System.Collections;
using Coffee_Rush.Board;
using UnityEngine;

namespace Coffee_Rush.Block
{
    public class BLockMatcher : MonoBehaviour
    {
        [Header("Anim Config")]
        [SerializeField] private float moveDuration;
        
        [Header("Matching Settings")]
        [SerializeField] private int currEmptyIdx;
        
        public bool MatchingAllowed {get; set;}
        

        public IEnumerator TryCollectGateItem(Collision2D other, eColorType colorType, CupHolder[] cupHolders)
        {
            MatchingAllowed = true;
            if (currEmptyIdx < cupHolders.Length && other.gameObject.TryGetComponent(out GateController gateController))
            {
                while (gateController.ColorType == colorType)
                {
                    if (!MatchingAllowed) yield break;
                    
                    GateItem item = gateController.GetMatchedItem();

                    if (!item) yield break;
                    
                    cupHolders[currEmptyIdx++].AttractGateItem(item);
                    yield return WaitHelper.GetWait(moveDuration);
                }
            }
        }
    }
}