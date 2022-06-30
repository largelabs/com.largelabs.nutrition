using System;
using UnityEngine;

[Serializable]
public class Frame
{
	[SerializeField] private Sprite sprite;
	[SerializeField] private float screenTime;

	private float currentTime = 0;

	public bool IsFinishedPlaying() => screenTime <= currentTime;

	public void IncrementCurrentTime(float increaseValue) => currentTime += increaseValue;

	public Sprite GetSprite() => sprite;
}
