public interface IFrameSwapper
{
	float AnimationSpeedMultiplier { get; set; }

	int LoopCount { get; }

	bool IsPlaying { get; }
}