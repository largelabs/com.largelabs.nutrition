using UnityEngine;

public interface IFrame
{
	float CurrentFrameTime { get; }

	float CurrentFrameRatio { get; }

	bool IsFinishedPlaying { get; }

	Sprite FrameSprite { get; }
}