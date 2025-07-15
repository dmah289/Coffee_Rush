using Coffee_Rush.Board;
using UnityEngine;

namespace Coffee_Rush.Block
{
    public class BLockMatcher : MonoBehaviour
    {
        [Header("Matching Settings")]
        [SerializeField] private eColorType colorType;
        [SerializeField] private CupHolder[] cupHolders;
        [SerializeField] private int currEmptyIdx;
        

        public void TryCollectGateItem(Collision2D other)
        {
            if (other.gameObject.TryGetComponent(out GateController gateController))
            {
                while (gateController.GetColorTypeFirstItem == colorType)
                {
                    GateItem item = gateController.CollectFirstGateItem();
                    cupHolders[currEmptyIdx++].AttractGateItem(item);
                }
            }
        }
    }
}