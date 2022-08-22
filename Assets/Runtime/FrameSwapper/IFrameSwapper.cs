public interface IFrameSwapper
{
    float AnimationSpeedMultiplier { get; }

	int LoopCount { get; }

	bool IsPlaying { get; }

	bool IsPaused { get; }
}
