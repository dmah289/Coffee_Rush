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
        [SerializeField] private eColorType colorType;
        [SerializeField] private CupHolder[] cupHolders;
        [SerializeField] private int currEmptyIdx;
        
        public bool MatchingAllowed {get; set;}

        private void Awake()
        {
            cupHolders = GetComponentsInChildren<CupHolder>();
        }

        private void Start()
        {
            Setup();
        }

        public void Setup()
        {
            for (int i = 0; i < cupHolders.Length; i++)
            {
                cupHolders[i].Setup(colorType);
            }
        }
        

        public IEnumerator TryCollectGateItem(Collision2D other)
        {
            MatchingAllowed = true;
            if (currEmptyIdx < cupHolders.Length && other.gameObject.TryGetComponent(out GateController gateController))
            {
                while (gateController.ColorType == colorType)
                {
                    if (!MatchingAllowed) yield break;
                    
                    GateItem item = gateController.GetMatchedItem();

                    if (item == null) yield break;
                    
                    cupHolders[currEmptyIdx++].AttractGateItem(item);
                    yield return WaitHelper.GetWait(moveDuration);
                }
            }
        }
    }
}