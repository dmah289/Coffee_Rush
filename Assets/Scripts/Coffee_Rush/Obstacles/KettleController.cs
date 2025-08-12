using System;
using Coffee_Rush.Block;
using Coffee_Rush.Level;
using Coffee_Rush.UI.InGame;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Framework.ObjectPooling;
using TMPro;
using UnityEngine;

namespace Coffee_Rush.Obstacles
{
    public class KettleController : MonoBehaviour
    {
        [SerializeField] private TextMeshPro countdownTxt;

        private int kettleCountdown;
        public int KettleCountDown
        {
            get => kettleCountdown;
            set
            {
                if (value > 0)
                {
                    kettleCountdown = value;
                    countdownTxt.text = $"{kettleCountdown}";
                }
                else
                {
                    kettleCountdown = 0;
                    transform.DOScale(new Vector3(2f, 2f, 2f), 1f).SetDelay(1f).OnComplete(() =>
                    {
                        LevelManager.Instance.ShowLoosePanel(eLooseReason.KettleExplosion);
                    });
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
            transform.localScale = Vector3.one;
        }

        public void ReturnToPool()
        {
            ObjectPooler.ReturnToPool(PoolingType.Kettle, this);
        }
    }
}