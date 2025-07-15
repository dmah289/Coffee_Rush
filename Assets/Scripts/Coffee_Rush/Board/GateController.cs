using System.Collections.Generic;
using UnityEngine;

namespace Coffee_Rush.Board
{
    public class GateController : MonoBehaviour
    {
        [Header("Manager")] 
        [SerializeField] private List<GateItem> gateItems;

        public eColorType GetColorTypeFirstItem
        {
            get
            {
                if (gateItems.Count > 0)
                {
                    return gateItems[0].ColorType;
                }

                return eColorType.None;
            }
        }

        public GateItem CollectFirstGateItem()
        {
            if (gateItems.Count > 0)
            {
                GateItem item = gateItems[0];
                gateItems.RemoveAt(0);
                return item;
            }

            return null;
        }
    }
}