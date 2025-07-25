using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee_Rush.Level
{
    public class LevelTimer : MonoBehaviour
    {
        private static readonly int FillAmount = Shader.PropertyToID("_FillAmount");

        [Header("Self Components")]
        [SerializeField] private Text timerTxt;
        [SerializeField] private Image timerBg;

        private CancellationTokenSource cts;
        private bool isTimerRunning;
        private float curTime;
        private float totalTime;
        
        public float CurTime
        {
            get => curTime;
            set
            {
                if (value > 0f)
                {
                    curTime = value;
                    float minutes = Mathf.Floor(value / 60);
                    float seconds = value % 60;
                    timerTxt.text = $"{minutes:00}:{seconds:00}";
                }
                else
                {
                    curTime = 0f;
                    timerTxt.text = "00:00";
                    LevelManager.Instance.FailLevel();
                }
            }
        }
        
        public void Setup(float totalTime)
        {
            this.totalTime = totalTime;
            CurTime = totalTime;
            cts = new CancellationTokenSource();
        }
        
        public void StartTimerOnFirstBlockMove()
        {
            if (!isTimerRunning && curTime > 0)
            {
                RunTimerAsync().Forget();
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) PauseTimer();
            else ResumeTimer();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if(!hasFocus) PauseTimer();
            else ResumeTimer();
        }

        private void ResumeTimer()
        {
            if(!isTimerRunning && curTime > 0)
                RunTimerAsync().Forget();
        }

        private void PauseTimer()
        {
            if (isTimerRunning)
            {
                isTimerRunning = false;
                cts.Cancel();
                cts.Dispose();
                cts = new CancellationTokenSource();
            }
        }

        private async UniTaskVoid RunTimerAsync()
        {
            isTimerRunning = true;

            try
            {
                while (curTime > 0 && !cts.IsCancellationRequested)
                {
                    await UniTask.Delay(50, cancellationToken: cts.Token);
                    CurTime -= 0.05f;
                    // SetElapsedTimer();
                }
            }
            catch (OperationCanceledException){}
        }

        private void SetElapsedTimer()
        {
            timerBg.material.SetFloat(FillAmount, curTime / totalTime);
        }

        public void ResetTimer()
        {
            PauseTimer();
            CurTime = totalTime;
        }
    }
}