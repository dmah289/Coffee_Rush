using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee_Rush.Level
{
    public class LevelTimer : MonoBehaviour
    {
        private static readonly int OutlineColorProperty = Shader.PropertyToID("_OutlineColor");
        private static readonly int ProgressProperty = Shader.PropertyToID("_Progress");

        [Header("Self Components")]
        [SerializeField] private Text timerTxt;
        [SerializeField] private Image timerBg;
        
        [Header("Timer colors")]
        [SerializeField] private Color[] timerColors;
        private int currColorIdx;

        private CancellationTokenSource cts;
        private bool isTimerRunning;
        private bool hasStarted;
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
                    float seconds = Mathf.Floor(value % 60);
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
            hasStarted = false;
            isTimerRunning = false;
            
            currColorIdx = 0;
            SetTimerOutline(1f);
            
            cts = new CancellationTokenSource();
        }
        
        public void StartTimerOnFirstBlockMove()
        {
            if (!isTimerRunning && curTime > 0)
            {
                hasStarted = true;
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
            if(hasStarted && !isTimerRunning && curTime > 0)
                RunTimerAsync().Forget();
        }

        private void PauseTimer()
        {
            if (hasStarted && isTimerRunning)
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
                    SetTimerOutline(curTime / totalTime);
                }
            }
            catch (OperationCanceledException){}
        }

        private void SetTimerOutline(float progress)
        {
            timerBg.material.SetFloat(ProgressProperty, progress);

            progress -= 0.01f;
            if (currColorIdx != (int)(progress * 4))
            {
                currColorIdx = (int)(progress * 4);
                timerBg.material.SetColor(OutlineColorProperty, timerColors[currColorIdx]);
            }
        }

        public void ResetTimer()
        {
            PauseTimer();
            CurTime = totalTime;
            SetTimerOutline(1f);
        }
    }
}