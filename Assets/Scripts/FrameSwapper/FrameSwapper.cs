using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FrameSwapper : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;
	private bool isPlaying = true;

	[SerializeField] private bool isLooping;
	[SerializeField] private Frame[] frames;

	private Frame currentFrame;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		currentFrame = frames[0];
	}

	private void Update()
	{
		if (isPlaying && currentFrame.IsFinishedPlaying())
		{
			UpdateCurrentFrame();
		}

		currentFrame.IncrementCurrentTimeSpent(Time.deltaTime);
		spriteRenderer.sprite = currentFrame.GetSprite();
	}

	private void UpdateCurrentFrame()
	{
		if (LastFrameReached)
		{
			return;
		}

		currentFrame = CollectionUtilities.GetNextElementInCircularCollection(currentFrame, frames);

		currentFrame.ResetCurrentTimeSpent();
	}

	private bool LastFrameReached => !isLooping && frames.Last().Equals(currentFrame);

	public void Pause() => isPlaying = false;

	public void Resume() => isPlaying = true;

	public void ResetAnimation() => currentFrame = frames[0];

}
