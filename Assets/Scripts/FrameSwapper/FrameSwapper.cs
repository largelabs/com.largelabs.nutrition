using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IFrameSwapper
{
	// Define user's API here
}


public class FrameSwapper<TRenderer> : MonoBehaviour, IFrameSwapper
{
	[SerializeField] bool playOnEnable = false;
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private bool isLooping;
	[SerializeField] private List<Frame> frames;
	[SerializeField] private int loopIndex;
	[SerializeField] float animationSpeedMultiplier = 1;

	bool isResumed = true;
	private Frame currentFrame;
	private Coroutine playback = null;

	private int loopCount = 0;
;
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
		playback = StartCoroutine(playbackRoutine());

	}

	public void Stop()
    {
		if (null == playback) return;

		StopCoroutine(playback);
		playback = null;
	}

	public bool IsPlaying => null != playback;


	public void Pause() => isResumed = false;

	public void Resume() => isResumed = true;


	// make it while loop with yielding
	// don't forget to call Stop here when the anim ends, to release the routine ref
	private IEnumerator playbackRoutine()
	{

		while(true)
        {
			if (isResumed && currentFrame.IsFinishedPlaying)
			{
				UpdateCurrentFrame();
			}

			currentFrame.IncrementCurrentTimeSpent(Time.deltaTime * animationSpeedMultiplier);
			spriteRenderer.sprite = currentFrame.FrameSprite;

			yield return null;
		}



	}

	private void UpdateCurrentFrame()
	{
		if (LastFrameReached)
		{
			Stop();
			return;
		}

		currentFrame = CollectionUtilities.GetNextElementInCircularCollection(currentFrame, frames);

		currentFrame.ResetCurrentTimeSpent();
	}

	private bool LastFrameReached => !isLooping && frames.Last().Equals(currentFrame);


	public void ResetAnimation() => currentFrame = frames[0];

}
