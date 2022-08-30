using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameTimer : MonoBehaviourBase
{
    [SerializeField] [Range(10, 500)] private int timeSeconds = 200;
    [SerializeField] [Range(0f, 1f)] private float timerScale = 1f;

    public delegate void TimerEventDelegate(int i_totalSeconds);
    public TimerEventDelegate OnTimerStarted = null;
    public TimerEventDelegate OnTimerUpdated = null;
    public TimerEventDelegate OnTimerEnded = null;

    #region UNITY AND CORE

    void OnDestroy()
    {
        Reset();
    }

    public void Reset()
    {
        OnTimerEnded = null;
        OnTimerStarted = null;
        OnTimerUpdated = null;
    }

    #endregion

}
