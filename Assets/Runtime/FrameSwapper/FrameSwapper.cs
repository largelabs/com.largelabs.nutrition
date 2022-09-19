using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FrameSwapper<TRenderer, TFrame> : MonoBehaviourBase, IFrameSwapper where TRenderer : Component
{
	[SerializeField] private bool playOnEnable = false;
	[SerializeField] new protected TRenderer renderer;
	[SerializeField] private bool isLooping = false;
	[SerializeField] private bool resetOnLastFrame = false;
	[SerializeField] private List<Frame<TFrame>> frames;
	[SerializeField] private int loopStartIndex;

    private bool isResumed = true;
    protected Frame<TFrame> currentFrame;
    private Coroutine playback = null;
    private int loopCount = 0;
    private CycleEvent cycleEvent = null;
    private Frame<TFrame> lastFrame;
    private float animationSpeedMultiplier = 1;

    #region UNITY AND CORE

    protected override void Awake()
    {
        setCurrentFrame(0);
        lastFrame = frames[frames.Count - 1];
    }

    private void OnEnable()
    {
        if (playOnEnable) Play();
    }

    #endregion

    #region IFrameSwapper
    public float AnimationSpeedMultiplier => animationSpeedMultiplier;
	public int LoopCount => loopCount;
	public bool IsPlaying => null != playback;
	public bool IsPaused => !isResumed;
    #endregion

    #region MUTABLE

    public void RegisterCycleEvents(params CycleEvent[] i_cycleEvents)
	{
		foreach (CycleEvent eventCallback in i_cycleEvents)
		{
			cycleEvent += eventCallback;
		}
	}

	public void UnregisterCycleEvents(params CycleEvent[] i_cycleEvents)
	{
		foreach (CycleEvent eventCallback in i_cycleEvents)
		{
			cycleEvent -= eventCallback;
		}
	}

	public void UnregisterAllCycleEvents()
	{
		cycleEvent = null;
	}

    [ExposePublicMethod]
	public void Play()
	{
		if (playback != null) return;

        Resume();
        loopCount = 0;
		playback = StartCoroutine(playbackRoutine());
	}

    [ExposePublicMethod]
    public void Stop()
	{
		if (null == playback) return;

        this.DisposeCoroutine(ref playback);
        ResetAnimation();
    }

    [ExposePublicMethod]
    public void Pause() => isResumed = false;

    [ExposePublicMethod]
    public void Resume() => isResumed = true;

	public void ResetAnimation() => currentFrame = frames[0];

	public void StopLoop() => isLooping = false;

	public void StartLoop() => isLooping = true;

    public void SetAnimationSpeed(float value) => animationSpeedMultiplier = value;

    public void IncreaseAnimationSpeed(float increaseValue) => animationSpeedMultiplier += increaseValue;

    public void DecreaseAnimationSpeed(float decreaseValue) => animationSpeedMultiplier -= decreaseValue;

    #endregion

    #region PRIVATE

    private IEnumerator playbackRoutine()
    {
        while (true)
        {
            if (isResumed)
            {
                currentFrame.IncrementCurrentTimeSpent(Time.deltaTime * animationSpeedMultiplier);
                //Debug.LogError(currentFrame.FrameObject + " " + currentFrame.CurrentFrameTime);

                if (currentFrame.IsFinishedPlaying)
                {

                    updateCurrentFrame();
                }
            }

            updateRenderedObject();

            yield return null;
        }

    }

    private void setCurrentFrame(int i_index)
    {
        if (frames.Count > 0 && i_index < frames.Count)
        {
            currentFrame = frames[i_index];
        }
    }

    protected abstract void updateRenderedObject();

    private void updateCurrentFrame()
    {
        currentFrame.InvokeEndedPlaybackEvent();
        currentFrame.ResetCurrentTimeSpent();

        if (lastFrameReached)
        {
            this.DisposeCoroutine(ref playback);

            if (resetOnLastFrame)
                ResetAnimation();

            return;
        }

        currentFrame = CollectionUtilities.GetNextElementInCircularList(currentFrame, frames);

        currentFrame.InvokeStartPlaybackEvent();

        if (frames[0].Equals(currentFrame))
        {
            loopCount++;
            cycleEvent?.Invoke(this);
        }

        currentFrame.ResetCurrentTimeSpent();
    }

    private bool lastFrameReached => !isLooping && lastFrame.Equals(currentFrame);

    #endregion

}
