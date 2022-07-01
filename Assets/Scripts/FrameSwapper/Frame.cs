using System;
using UnityEngine;

[Serializable]
public class Frame : IFrame
{
	[SerializeField] private Sprite sprite;
	[SerializeField] private float screenTime;

	// Implement frame event register / unregister API and call from frame swapper
	FrameEvent OnStartedPlayback = null;
	FrameEvent OnEndedPlayback = null;

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

    #endregion
}
