using System;
using UnityEngine;

public class MinigameTimer : MonoBehaviourBase
{
    [SerializeField] [Range(1, 500)] private float timeSeconds = 200f;

    enum TimerStatus { Running, Paused, Ended, None };
    TimerStatus timerStatus = TimerStatus.None;

    public Action<TimeSpan> OnTimerStarted = null;
    public Action OnTimerEnded = null;
    public Action<TimeSpan> OnTimerPaused = null;
    public Action<TimeSpan> OnTimerResumed = null;
    public Action<float> OnAddedSeconds = null;
    public Action<string, string> OnDisplayUpdateRequest = null;

    TimeSpan remainingTime;

    #region UNITY AND CORE

    void OnDestroy()
    {
        Reset();
    }

    public void Reset()
    {
        OnTimerEnded = null;
        OnTimerStarted = null;
        OnTimerPaused = null;
        OnTimerResumed = null;
    }

    private void Update()
    {
        updateCountdown();
    }

    #endregion

    #region PUBLIC API

    public void ResetTimer()
    {
        timerStatus = TimerStatus.None;
        timeSeconds = 0f;
    }

    [ExposePublicMethod]
    public void StartTimer()
    {
        if (timerStatus != TimerStatus.None) return;

        timerStatus = TimerStatus.Running;

        OnTimerStarted?.Invoke(remainingTime);
    }

    [ExposePublicMethod]
    public void PauseTimer()
    {
        if (timerStatus == TimerStatus.Paused) return;

        timerStatus = TimerStatus.Paused;

        OnTimerPaused?.Invoke(remainingTime);
    }

    [ExposePublicMethod]
    public void ResumeTimer()
    {
        if (timerStatus != TimerStatus.Paused) return;

        timerStatus = TimerStatus.Running;

        OnTimerResumed?.Invoke(remainingTime);
    }

    public string GetMinutesString()
    {
        return remainingTime.Minutes.ToString("00");
    }

    public string GetSecondsString()
    {
        return remainingTime.Seconds.ToString("00");
    }

    public void SetTimer(float i_time, bool i_updateDisplay)
    {
        timeSeconds = i_time;
        remainingTime = TimeSpan.FromSeconds(timeSeconds);
        if(true == i_updateDisplay)
        {
            updateDisplay();
        }
    }  
    
    public float RemainingTimeSeconds => remainingTime.Seconds;

    [ExposePublicMethod]
    public void AddTime(float i_timeBonus)
    {
        SetTimer(timeSeconds + i_timeBonus, true);
        OnAddedSeconds?.Invoke(i_timeBonus);
    }

    #endregion

    #region PRIVATE API

    private void updateCountdown()
    {
        if (timerStatus != TimerStatus.Running) return;

        if (lastDisplayUpdateTime.Seconds != remainingTime.Seconds)
            updateDisplay();

        SetTimer(timeSeconds - Time.unscaledDeltaTime, false);

        if (timeSeconds <= 0) onTimeEnd();
    }

    private void onTimeEnd()
    {
        timeSeconds = 0;
        timerStatus = TimerStatus.Ended;
        OnTimerEnded?.Invoke();
    }

    TimeSpan lastDisplayUpdateTime;

    void updateDisplay()
    {
        OnDisplayUpdateRequest?.Invoke(GetMinutesString(), GetSecondsString());
        lastDisplayUpdateTime = remainingTime;
    }

    #endregion

}
