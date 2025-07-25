using System;
using Coffee_Rush.Block;
using Coffee_Rush.Level;
using Framework.ObjectPooling;
using TMPro;
using UnityEngine;

namespace Coffee_Rush.Obstacles
{
    public class KettleController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI countdownTxt;

        public int KettleCountDown
        {
            get
            {
                if(int.TryParse(countdownTxt.text, out int countdown))
                    return countdown;

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    countdownTxt.text = value.ToString();
                }
                else
                {
                    countdownTxt.text = string.Empty;
                    LevelManager.Instance.FailLevel();
                }
            }
        }

        private void OnEnable()
        {
            BLockMatcher.OnBlockFullSlot += OnBlockColected;
        }

        private void OnDisable()
        {
            BLockMatcher.OnBlockFullSlot -= OnBlockColected;
        }

        public void OnBlockColected()
        {
            KettleCountDown--;
        }

        public void Setup(Vector3 pos, int countdown)
        {
            transform.position = pos;
            KettleCountDown = countdown;
        }

        public void OnRevokenToPool()
        {
            ObjectPooler.ReturnToPool(PoolingType.Kettle, this);
        }
    }
}