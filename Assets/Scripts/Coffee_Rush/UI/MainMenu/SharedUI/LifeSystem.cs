using System;
using Framework;
using Framework.DesignPattern;
using TMPro;
using UnityEngine;

namespace Coffee_Rush.UI.MainMenu.Home
{
    public class LifeSystem : MonoSingleton<LifeSystem>
    {
        [Header("Self Components")]
        [SerializeField] private TextMeshProUGUI counter;
        [SerializeField] private TextMeshProUGUI timer;

        
        private DateTime lastSaveTime;
        private bool isTimerRunning;
        private float timeForOneLife = 1800;
        private int maxLives = 5;
        
        private int curLife;
        public int CurLife
        {
            get => curLife;
            set
            {
                curLife = Mathf.Clamp(value, 0, maxLives);
                UpdateLifeCounterDisplay();
            }
        }

        private float countdownRemaining;
        public float CurCountdown
        {
            get => countdownRemaining;
            set
            {
                countdownRemaining = Mathf.Max(value, 0);
                UpdateTimerDisplay();
            }
        }
        
        public bool CanPlay => CurLife > 0;

        private void OnEnable()
        {
            LoadLifeData();
        }

        private void Update()
        {
            if (isTimerRunning)
            {
                CurCountdown -= Time.deltaTime;

                if (countdownRemaining <= 0)
                {
                    CurLife++;
                    if(CurLife < maxLives) CurCountdown = timeForOneLife;
                }
            }
        }
        
        private void OnDisable()
        {
            SaveLifeData();
        }
        
        private void OnApplicationQuit()
        {
            SaveLifeData();
        }
        
        private void LoadLifeData()
        {
            CurLife = 0;
            CurCountdown = timeForOneLife;
            if (PlayerPrefs.HasKey(KeySave.lastSaveTimeKey))
            {
                DateTime savedTime = DateTime.Parse(PlayerPrefs.GetString(KeySave.lastSaveTimeKey));
                TimeSpan elapsedTime = DateTime.Now - savedTime;
                countdownRemaining = PlayerPrefs.GetFloat(KeySave.lastCountdownRemaining);
                
                float totalSecondsElapsed = (float)elapsedTime.TotalSeconds;
                int livesToAdd = Mathf.FloorToInt(totalSecondsElapsed / timeForOneLife);
        
                if (livesToAdd > 0)
                {
                    CurLife += livesToAdd;
                    CurCountdown = timeForOneLife - totalSecondsElapsed % timeForOneLife;
                }
                else
                {
                    CurCountdown -= totalSecondsElapsed;
                }
            }
        }
        
        private void SaveLifeData()
        {
            PlayerPrefs.SetInt(KeySave.curLifeKey, CurLife);
            PlayerPrefs.SetString(KeySave.lastSaveTimeKey, DateTime.Now.ToString());
            PlayerPrefs.SetFloat(KeySave.lastCountdownRemaining, countdownRemaining);
            PlayerPrefs.Save();
        }

        private void UpdateLifeCounterDisplay()
        {
            counter.text = $"{curLife}";

            if (curLife == maxLives)
            {
                isTimerRunning = false;
                timer.text = KeySave.maxKey;
            }
            else isTimerRunning = true;
        }

        private void UpdateTimerDisplay()
        {
            if (curLife < maxLives)
            {
                int minutes = Mathf.FloorToInt(countdownRemaining / 60);
                int seconds = Mathf.FloorToInt(countdownRemaining % 60);
                timer.text = $"{minutes:00}:{seconds:00}";
            }
        }
    }
}