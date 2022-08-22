public interface IFrameSwapper
{
    // Code review comments : no {get; set;}. Please create a private variable and create a property get for it
    // and a setter in the mutable public API -> keep this interface read-only
    float AnimationSpeedMultiplier { get; set; }

	int LoopCount { get; }

	bool IsPlaying { get; }

	bool IsPaused { get; }
}
