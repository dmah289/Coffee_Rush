using TMPro;
using UnityEngine;
using System;

namespace Coffee_Rush.UI.MainMenu.Home
{
    public class Test : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private TextMeshProUGUI counter;
        [SerializeField] private TextMeshProUGUI timer;

        [Header("Settings")]
        [SerializeField] private int maxLives = 5;
        [SerializeField] private float timeForOneLife = 1800f; // 30 minutes in seconds

        private int curLife;
        private float timeRemaining;
        private DateTime lastSaveTime;
        private bool isTimerRunning;

        public int CurLife
        {
            get => curLife;
            set
            {
                curLife = Mathf.Clamp(value, 0, maxLives);
                OnCurLifeChanged();
            }
        }

        private void Start()
        {
            LoadLifeData();
            UpdateUI();
        }

        private void OnApplicationPause(bool isPaused)
        {
            if (isPaused)
            {
                SaveLifeData();
            }
            else
            {
                LoadLifeData();
            }
        }

        private void OnApplicationQuit()
        {
            SaveLifeData();
        }

        private void Update()
        {
            if (isTimerRunning)
            {
                timeRemaining -= Time.deltaTime;
                
                if (timeRemaining <= 0)
                {
                    AddLife();
                    if (curLife < maxLives)
                    {
                        ResetTimer();
                    }
                }
                
                UpdateTimerDisplay();
            }
        }

        private void OnCurLifeChanged()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            counter.text = curLife == maxLives ? "MAX" : curLife.ToString();
            
            if (curLife < maxLives)
            {
                RunTimer();
            }
            else
            {
                StopTimer();
                timer.text = "";
            }
        }

        private void RunTimer()
        {
            isTimerRunning = true;
            UpdateTimerDisplay();
        }

        private void StopTimer()
        {
            isTimerRunning = false;
        }

        private void ResetTimer()
        {
            timeRemaining = timeForOneLife;
        }

        private void UpdateTimerDisplay()
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        private void AddLife()
        {
            if (curLife < maxLives)
            {
                CurLife++;
            }
        }

        private void SaveLifeData()
        {
            PlayerPrefs.SetInt("Lives", curLife);
            PlayerPrefs.SetString("LastSaveTime", DateTime.Now.ToString("o"));
            PlayerPrefs.SetFloat("TimeRemaining", timeRemaining);
            PlayerPrefs.Save();
        }

        private void LoadLifeData()
        {
            if (PlayerPrefs.HasKey("Lives"))
            {
                curLife = PlayerPrefs.GetInt("Lives");
                
                if (PlayerPrefs.HasKey("LastSaveTime"))
                {
                    DateTime savedTime = DateTime.Parse(PlayerPrefs.GetString("LastSaveTime"));
                    TimeSpan elapsedTime = DateTime.Now - savedTime;
                    
                    timeRemaining = PlayerPrefs.GetFloat("TimeRemaining");
                    
                    // Calculate lives gained during absence
                    float totalSecondsElapsed = (float)elapsedTime.TotalSeconds;
                    int livesToAdd = Mathf.FloorToInt(totalSecondsElapsed / timeForOneLife);
                    
                    if (livesToAdd > 0)
                    {
                        curLife += livesToAdd;
                        curLife = Mathf.Clamp(curLife, 0, maxLives);
                        
                        // Calculate remaining time for next life
                        float remainingSeconds = totalSecondsElapsed % timeForOneLife;
                        timeRemaining = timeForOneLife - remainingSeconds;
                    }
                    else
                    {
                        // Just subtract elapsed time from timer
                        timeRemaining -= (float)elapsedTime.TotalSeconds;
                        if (timeRemaining < 0)
                        {
                            timeRemaining = 0;
                        }
                    }
                }
                else
                {
                    timeRemaining = timeForOneLife;
                }
            }
            else
            {
                curLife = maxLives;
                timeRemaining = timeForOneLife;
            }
            
            OnCurLifeChanged();
        }
    }
}