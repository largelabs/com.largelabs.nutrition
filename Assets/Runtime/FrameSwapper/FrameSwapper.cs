using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameSwapper : MonoBehaviourBase, IFrameSwapper
{
	#region Serialized fields
	[SerializeField] private bool playOnEnable = false;
	[SerializeField] private Renderer renderer;
	[SerializeField] private bool isLooping;
	[SerializeField] private List<Frame<Sprite>> frames;
	[SerializeField] private int loopIndex;
	#endregion

	#region IFrameSwapper
	public float AnimationSpeedMultiplier { get; set; }
	public int LoopCount => loopCount;
	public bool IsPlaying => null != playback;
	#endregion

	#region private members
	private bool isResumed = true;
	private Frame<Sprite> currentFrame;
	private Coroutine playback = null;
	private int loopCount = 0;
	private CycleEvent cycleEvent = null;
	private Frame<Sprite> lastFrame;
	#endregion

	protected override void Awake()
	{
		setCurrentFrame(0);
		lastFrame = frames[frames.Count - 1];
		AnimationSpeedMultiplier = 1;
	}

	private void OnEnable()
	{
		if (playOnEnable) Play();
	}

	private IEnumerator playbackRoutine()
	{

		while (true)
		{
			if (isResumed)
			{
				currentFrame.IncrementCurrentTimeSpent(Time.deltaTime * AnimationSpeedMultiplier);

				if (currentFrame.IsFinishedPlaying)
				{
					updateCurrentFrame();
				}
			}

			updateRenderedObject();

			yield return null;
		}

	}

	public void RegisterCycleEvents(params CycleEvent[] i_cycleEvents)
	{
		foreach (var eventCallback in i_cycleEvents)
		{
			cycleEvent += eventCallback;
		}
	}

	public void UnregisterCycleEvents(params CycleEvent[] i_cycleEvents)
	{
		foreach (var eventCallback in i_cycleEvents)
		{
			cycleEvent -= eventCallback;
		}
	}

	public void UnregisterAllCycleEvents()
	{
		cycleEvent = null;
	}

	#region helper functions
	public void Play()
	{
		if (playback != null) return;

		loopCount = 0;
		playback = StartCoroutine(playbackRoutine());

	}

	public void Stop()
	{
		if (null == playback) return;

		StopCoroutine(playback);
		playback = null;
	}

	public void Pause() => isResumed = false;

	public void Resume() => isResumed = true;

	public void ResetAnimation() => currentFrame = frames[0];

	#endregion

	private void setCurrentFrame(int i_index)
	{
		if (frames.Count > 0 && i_index < frames.Count)
		{
			currentFrame = frames[i_index];
		}
	}

	private void updateRenderedObject()
	{
		if (renderer is SpriteRenderer)
		{
			(renderer as SpriteRenderer).sprite = currentFrame.FrameObject;
		}
	}

	private void updateCurrentFrame()
	{
		if (lastFrameReached)
		{
			Stop();
			return;
		}

		currentFrame.InvokeEndedPlaybackEvent();

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

}
