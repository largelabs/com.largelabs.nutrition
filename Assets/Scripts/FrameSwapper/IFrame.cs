using UnityEngine;

public interface IFrame<T>
{
	float CurrentFrameTime { get; }

	float CurrentFrameRatio { get; }

	bool IsFinishedPlaying { get; }

	T FrameObject { get; }
}