using System;
using UnityEngine;

[Serializable]
public class Frame
{
	[SerializeField] private Sprite sprite;
	[SerializeField] private float screenTime;

	private float currentTimeSpent = 0;

	public bool IsFinishedPlaying() => screenTime <= currentTimeSpent;

	public void IncrementCurrentTimeSpent(float increaseValue) => currentTimeSpent += increaseValue;

	public void ResetCurrentTimeSpent() => currentTimeSpent = 0;

	public Sprite GetSprite() => sprite;
}
