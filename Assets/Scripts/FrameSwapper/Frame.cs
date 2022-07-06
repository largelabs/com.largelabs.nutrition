using System;
using UnityEngine;

[Serializable]
public class Frame<T> : IFrame<T>
{
	[SerializeField] private T frameObject;
	[SerializeField] private float screenTime;

	// Implement frame event register / unregister API and call from frame swapper

	private FrameEvent<T> onStartedPlayback = null;
	private FrameEvent<T> onEndedPlayback = null;


	private float currentTimeSpent = 0;

    #region IFrame

    public float CurrentFrameTime => currentTimeSpent;

	public float CurrentFrameRatio => currentTimeSpent / screenTime;

	public bool IsFinishedPlaying => screenTime <= currentTimeSpent;

	public T FrameObject => frameObject;

	#endregion

	#region MUTABLE

	public void IncrementCurrentTimeSpent(float i_increaseValue) => currentTimeSpent += i_increaseValue;

	public void ResetCurrentTimeSpent() => currentTimeSpent = 0;

	#region events registery and invocation
	public void RegisterStartPlaybackEvent(FrameEvent<T> i_frameEvent) => onStartedPlayback += i_frameEvent;
	public void UnRegisterStartPlaybackEvent(FrameEvent<T> i_frameEvent) => onStartedPlayback -= i_frameEvent;
	public void UnRegisterAllStartPlaybackEvents() => onStartedPlayback = null;

	public void RegisterEndedPlaybackEvent(FrameEvent<T> i_frameEvent) => onEndedPlayback += i_frameEvent;
	public void UnRegisterEndedPlaybackEvent(FrameEvent<T> i_frameEvent) => onEndedPlayback -= i_frameEvent;
	public void UnRegisterAllEndedPlaybackEvents() => onEndedPlayback = null;

	public void InvokeStartPlaybackEvent() => onStartedPlayback.Invoke(this);
	public void InvokeEndedPlaybackEvent() => onEndedPlayback.Invoke(this);
	#endregion

	#endregion
}
