using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee_Rush.Level
{
    public class LevelTimer : MonoBehaviour
    {
        private static readonly int OutlineColorProperty = Shader.PropertyToID("_OutlineColor");
        private static readonly int ProgressProperty = Shader.PropertyToID("_Progress");

        [Header("Self Components")]
        [SerializeField] private Text counter;
        [SerializeField] private Image timerBg;
        
        [Header("Timer colors")]
        [SerializeField] private Color[] timerColors;
        private int currColorIdx;
        [SerializeField] private Color waringColor;

        private CancellationTokenSource cts;
        private bool isTimerRunning;
        private bool hasStarted;
        private float countDownTimer;
        private float totalTime;
        private bool isFlashing;
        
        public float CountDownTimer
        {
            get => countDownTimer;
            set
            {
                if (value > 0f)
                {
                    countDownTimer = value;
                    float minutes = Mathf.Floor(value / 60);
                    float seconds = Mathf.Floor(value % 60);
                    counter.text = $"{minutes:00}:{seconds:00}";

                    if (countDownTimer < 20 && !isFlashing)
                    {
                        counter.color = waringColor;
                        isFlashing = true;
                        counter.rectTransform.DOScale(Vector3.one * 1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
                        counter.DOFade(1, 0.5f).SetLoops(-1, LoopType.Yoyo);
                    }
                    else if (countDownTimer >= 20 && isFlashing)
                    {
                        counter.color = Color.white;
                        isFlashing = false;
                        counter.rectTransform.DOKill();
                        counter.DOKill();
                    }
                }
                else
                {
                    countDownTimer = 0f;
                    counter.text = "00:00";
                    LevelManager.Instance.FailLevel();
                }
            }
        }

        public void Setup(float totalTime)
        {
            this.totalTime = totalTime;
            CountDownTimer = totalTime;
            hasStarted = false;
            isTimerRunning = false;
            currColorIdx = 0;
            
            SetTimerOutline(1f);
        }
        
        public void StartTimerOnFirstBlockMove()
        {
            if (!isTimerRunning && countDownTimer > 0)
            {
                hasStarted = true;
                cts = new CancellationTokenSource();
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
            if(hasStarted && !isTimerRunning && countDownTimer > 0)
                RunTimerAsync().Forget();
        }

        public void PauseTimer()
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
                while (countDownTimer > 0 && !cts.IsCancellationRequested)
                {
                    await UniTask.Delay(50, cancellationToken: cts.Token);
                    CountDownTimer -= 0.05f;
                    SetTimerOutline(countDownTimer / totalTime);
                }
            }
            catch (OperationCanceledException){}
        }

        private void SetTimerOutline(float progress)
        {
            timerBg.material.SetFloat(ProgressProperty, progress);

            if (currColorIdx != (int)(progress * 4))
            {
                currColorIdx = (int)(progress * 4);
                if (currColorIdx > 3) currColorIdx = 3;
                timerBg.material.SetColor(OutlineColorProperty, timerColors[currColorIdx]);
            }
        }
    }
}