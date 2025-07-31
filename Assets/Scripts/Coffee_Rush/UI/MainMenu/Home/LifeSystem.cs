using System;
using Framework;
using TMPro;
using UnityEngine;

namespace Coffee_Rush.UI.MainMenu.Home
{
    public class LifeSystem : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private TextMeshProUGUI counter;
        [SerializeField] private TextMeshProUGUI timer;

        
        private DateTime lastSaveTime;
        private bool isTimerRunning;
        private float timeForOneLife = 3;
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

        private float timeRemaining;
        public float CurTimeCounter
        {
            get => timeRemaining;
            set
            {
                timeRemaining = Mathf.Max(value, 0);
                UpdateTimerDisplay();
            }
        }

        private void Start()
        {
            // LoadLifeData();
            CurLife = 3;
        }

        private void Update()
        {
            if (isTimerRunning)
            {
                CurTimeCounter -= Time.deltaTime;

                if (timeRemaining <= 0)
                {
                    CurLife++;
                    if(CurLife < maxLives) timeRemaining = timeForOneLife;
                }
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if(pauseStatus) SaveLifeData();
            else LoadLifeData();
        }

        private void OnApplicationQuit()
        {
            SaveLifeData();
        }

        private void LoadLifeData()
        {
            CurLife = PlayerPrefs.GetInt(KeySave.curLifeKey, maxLives);
            timeRemaining = 0;
            if (PlayerPrefs.HasKey(KeySave.lastSaveTimeKey))
            {
                DateTime savedTime = DateTime.Parse(PlayerPrefs.GetString(KeySave.lastSaveTimeKey));
                TimeSpan elapsedTime = DateTime.Now - savedTime;

                timeRemaining = PlayerPrefs.GetFloat(KeySave.timeRemaining, 0);
                float totalSecondsElapsed = (float)elapsedTime.TotalSeconds + timeRemaining;
                int livesToAdd = Mathf.FloorToInt(totalSecondsElapsed / timeForOneLife);

                if (livesToAdd > 0)
                {
                    CurLife += livesToAdd;
                    timeRemaining = timeForOneLife - totalSecondsElapsed % timeForOneLife;
                }
            }
        }

        private void SaveLifeData()
        {
            PlayerPrefs.SetInt(KeySave.curLifeKey, CurLife);
            PlayerPrefs.SetString(KeySave.lastSaveTimeKey, DateTime.Now.ToString());
            PlayerPrefs.SetFloat(KeySave.timeRemaining, timeRemaining);
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
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timer.text = $"{minutes:00}:{seconds:00}";
        }
    }
}