using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FrameSwapper<TRenderer> : MonoBehaviourBase, IFrameSwapper where TRenderer : Renderer
{
	#region Serialized fields
	[SerializeField] private bool playOnEnable = false;
	[SerializeField] private TRenderer renderer;
	[SerializeField] private bool isLooping;
	[SerializeField] private List<Frame<Sprite>> frames;
	[SerializeField] private int loopIndex;
	#endregion

	#region IFrameSwapper
	public float AnimationSpeedMultiplier { get; set; }
	public int LoopCount => loopCount;
	public bool IsPlaying => null != playback;
	#endregion

	private bool isResumed = true;
	private Frame<Sprite> currentFrame;
	private Coroutine playback = null;
	private int loopCount = 0;
	private CycleEvent cycleEvent = null;

	private void Awake()
	{
		setCurrentFrame(0);
	}

	private void OnEnable()
	{
		if (playOnEnable) Play();
	}

	void setCurrentFrame(int i_index)
	{
		int count = frames.Count;
		if (count > 0 && i_index < count)
			currentFrame = frames[0];
	}

	public void Play()
	{
		if (playback != null) return;

		// Start animation
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


	// make it while loop with yielding
	// don't forget to call Stop here when the anim ends, to release the routine ref
	private IEnumerator playbackRoutine()
	{

		while (true)
		{
			if (isResumed)
			{
				currentFrame.IncrementCurrentTimeSpent(Time.deltaTime * AnimationSpeedMultiplier);

				if(currentFrame.IsFinishedPlaying)
				{
					updateCurrentFrame();
				}
			}

			updateRenderedObject();

			yield return null;
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
			cycleEvent.Invoke(this);
		}

		currentFrame.ResetCurrentTimeSpent();
	}

	private bool lastFrameReached => !isLooping && frames.Last().Equals(currentFrame);

	public void ResetAnimation() => currentFrame = frames[0];

}
