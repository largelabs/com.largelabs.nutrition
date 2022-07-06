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
	public void RegisterStartPlaybackEvents(params FrameEvent<T>[] i_frameEvents)
	{
		foreach(var frameEvent in i_frameEvents)
		{
			onStartedPlayback += frameEvent;
		}
	}

	public void UnRegisterStartPlaybackEvents(params FrameEvent<T>[] i_frameEvents)
	{
		foreach (var frameEvent in i_frameEvents)
		{
			onStartedPlayback -= frameEvent;
		}
	}

	public void UnRegisterAllStartPlaybackEvents() => onStartedPlayback = null;

	public void RegisterEndedPlaybackEvents(params FrameEvent<T>[] i_frameEvents)
	{
		foreach (var frameEvent in i_frameEvents)
		{
			onEndedPlayback += frameEvent;
		}
	}

	public void UnRegisterEndedPlaybackEvent(params FrameEvent<T>[] i_frameEvents)
	{
		foreach (var frameEvent in i_frameEvents)
		{
			onEndedPlayback -= frameEvent;
		}
	}

	public void UnRegisterAllEndedPlaybackEvents() => onEndedPlayback = null;

	public void InvokeStartPlaybackEvent() => onStartedPlayback?.Invoke(this);
	public void InvokeEndedPlaybackEvent() => onEndedPlayback?.Invoke(this);
	#endregion

	#endregion
}
