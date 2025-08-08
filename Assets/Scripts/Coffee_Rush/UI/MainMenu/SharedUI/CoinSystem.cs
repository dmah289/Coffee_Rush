using Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee_Rush.UI.MainMenu.SharedUI
{
    public class CoinSystem : MonoBehaviour
    {
        [SerializeField] private Text counterTxt;
        

        private int coinCount;
        public int CoinCounter
        {
            get => coinCount;
            set
            {
                coinCount = value;
                UpdateCounterDisplay();
            }
        }
        

        private void UpdateCounterDisplay()
        {
            counterTxt.text = $"{coinCount}";
        }

        private void OnEnable()
        {
            LoadCoinData();
        }

        private void OnDisable()
        {
            SaveCoinData();
        }

        private void OnApplicationQuit()
        {
            SaveCoinData();
        }

        private void LoadCoinData()
        {
            CoinCounter = PlayerPrefs.GetInt(KeySave.CoinCountKey, 0);
        }

        private void SaveCoinData()
        {
            PlayerPrefs.SetInt(KeySave.CoinCountKey, coinCount);
        }
    }
}