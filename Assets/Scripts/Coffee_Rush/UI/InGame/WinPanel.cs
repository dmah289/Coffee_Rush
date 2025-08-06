using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coffee_Rush.Level;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Framework.ObjectPooling;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Coffee_Rush.UI.InGame
{
    public class WinPanel : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private RectTransform selfRect;
        [SerializeField] private RectTransform bg_icon;
        [SerializeField] private RectTransform target;
        
        [Header("Claim Button Elements")]
        [SerializeField] private Text coinAmount;
        [SerializeField] private RectTransform coinIcon;
        [SerializeField] private RectTransform claimBtn;
        
        [Header("Manager")]
        [SerializeField] private List<RectTransform> coins;
        [SerializeField] private LevelTimer levelTimer;

        private void Awake()
        {
            coins = new List<RectTransform>();
        }

        private void Update()
        {
            Vector3 curRot = bg_icon.rotation.eulerAngles;
            curRot.z += 20 * Time.deltaTime;
            bg_icon.rotation = Quaternion.Euler(curRot);
        }

        public async UniTask Show()
        {
            levelTimer.PauseTimer();
            
            await UniTask.Delay(1000);
            
            gameObject.SetActive(true);

            coinAmount.text = $"{LevelManager.Instance.levelLoader.currLevelData.coinAmount}";
            selfRect.localScale = Vector3.zero;
            selfRect.DOScale(Vector3.one, 0.2f);
        }
        
        public void OnClaimBtnClicked() => OnClaimBtnClickedAsync().Forget();
        public async UniTask OnClaimBtnClickedAsync()
        {
            SpawnCoins();
            
            await CollectCoins();
            
            LevelManager.Instance.WinLevel();
            gameObject.SetActive(false);
        }

        private async UniTask CollectCoins()
        {
            Sequence sequence = DOTween.Sequence();
            for (int i = 0; i < coins.Count; i++)
            {
                sequence.Join(coins[i].DOMove(target.position, 1.5f).SetEase(Ease.InBack).SetDelay(0.05f*i));
            }

            await sequence.AsyncWaitForCompletion();
    
            for (int i = 0; i < coins.Count; i++)
            {
                ObjectPooler.ReturnToPool(PoolingType.Coin, coins[i]);
            }
            coins.Clear();
            
            await UniTask.Delay(1000);
        }

        private void SpawnCoins()
        {
            int amount = Random.Range(5, 7);
            coins.Clear();
            for (int i = 0; i < amount; i++)
            {
                coins.Add(ObjectPooler.GetFromPool<RectTransform>(PoolingType.Coin, claimBtn));
                Vector2 offset = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
                coins[i].anchoredPosition = coinIcon.anchoredPosition + offset;
            }
        }
    }
}