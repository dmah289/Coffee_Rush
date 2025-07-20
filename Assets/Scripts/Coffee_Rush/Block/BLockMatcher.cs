using System;
using System.Collections;
using Coffee_Rush.Board;
using Coffee_Rush.Gate;
using UnityEngine;

namespace Coffee_Rush.Block
{
    public class BLockMatcher : MonoBehaviour
    {
        [Header("Matching Settings")]
        [SerializeField] private int currEmptyIdx;
        
        
        public bool MatchingAllowed {get; set;}
        

        public IEnumerator TryCollectGateItem(Collider2D other, eColorType colorType, CupHolder[] cupHolders)
        {
            MatchingAllowed = true;
            
            if (other.gameObject.TryGetComponent(out GateController gateController) && currEmptyIdx < cupHolders.Length)
            {
                yield return null;
                
                while (currEmptyIdx < cupHolders.Length && gateController.ColorType == colorType)
                {
                    if (!MatchingAllowed) yield break;
                    
                    GateItem item = gateController.GetMatchedItem();

                    if (!item) yield break;
                    
                    yield return cupHolders[currEmptyIdx++].AttractGateItem(item);

                    yield return null;
                }
            }
        }
    }
}