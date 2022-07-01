using System;
using UnityEngine;

[Serializable]
public class Frame : IFrame
{
	[SerializeField] private Sprite sprite;
	[SerializeField] private float screenTime;

	// Implement frame event register / unregister API and call from frame swapper

	private FrameEvent onStartedPlayback = null;
	private FrameEvent onEndedPlayback = null;


	private float currentTimeSpent = 0;

    #region IFrame

    public float CurrentFrameTime => currentTimeSpent;

	public float CurrentFrameRatio => currentTimeSpent / screenTime;

	public bool IsFinishedPlaying => screenTime <= currentTimeSpent;

	public Sprite FrameSprite => sprite;

	#endregion

	#region MUTABLE

	public void IncrementCurrentTimeSpent(float increaseValue) => currentTimeSpent += increaseValue;

	public void ResetCurrentTimeSpent() => currentTimeSpent = 0;

	#region events registery and invocation
	public void RegisterStartPlaybackEvent(FrameEvent i_frameEvent) => onStartedPlayback += i_frameEvent;
	public void UnRegisterStartPlaybackEvent(FrameEvent i_frameEvent) => onStartedPlayback -= i_frameEvent;
	public void UnRegisterAllStartPlaybackEvents() => onStartedPlayback = null;

	public void RegisterEndedPlaybackEvent(FrameEvent i_frameEvent) => onEndedPlayback += i_frameEvent;
	public void UnRegisterEndedPlaybackEvent(FrameEvent i_frameEvent) => onEndedPlayback -= i_frameEvent;
	public void UnRegisterAllEndedPlaybackEvents() => onEndedPlayback = null;

	public void InvokeStartPlaybackEvent() => onStartedPlayback.Invoke(this);
	public void InvokeEndedPlaybackEvent() => onEndedPlayback.Invoke(this);
	#endregion

	#endregion
}
