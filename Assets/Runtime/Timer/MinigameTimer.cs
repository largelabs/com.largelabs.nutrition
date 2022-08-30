using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameTimer : MonoBehaviourBase
{
    [SerializeField] [Range(1, 500)] private float timeSeconds = 200f;

    enum TimerStatus { Running, Paused, Ended, None };
    TimerStatus timerStatus = TimerStatus.None;

    public delegate void TimerEventDelegate();

    public TimerEventDelegate OnTimerStarted = null;
    public TimerEventDelegate OnTimerEnded = null;
    public TimerEventDelegate OnTimerPaused = null;
    public TimerEventDelegate OnTimerResumed = null;


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

    [ExposePublicMethod]
    public void StartTimer()
    {
        if (timerStatus != TimerStatus.None) return;

        timerStatus = TimerStatus.Running;

        OnTimerStarted?.Invoke();
    }

    [ExposePublicMethod]
    public void PauseTimer()
    {
        if (timerStatus == TimerStatus.Paused) return;

        timerStatus = TimerStatus.Paused;

        OnTimerPaused?.Invoke();
    }

    [ExposePublicMethod]
    public void ResumeTimer()
    {
        if (timerStatus != TimerStatus.Paused) return;

        timerStatus = TimerStatus.Running;

        OnTimerResumed?.Invoke();
    }

    public string DisplayTimer()
    {

        return TimeSpan.FromSeconds(timeSeconds).ToString("ss\\.ff");
    }

    #endregion

    #region PRIVATE API

    private void updateCountdown()
    {
        if (timerStatus != TimerStatus.Running) return;

        timeSeconds -= Time.unscaledDeltaTime;

        if (timeSeconds <= 0) onTimeEnd();
    }

    private void onTimeEnd()
    {
        timeSeconds = 0;
        timerStatus = TimerStatus.Ended;
        OnTimerEnded?.Invoke();
    }

    #endregion

}
