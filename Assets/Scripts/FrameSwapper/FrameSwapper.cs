using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FrameSwapper : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;
	private bool isResumed = true;

	[SerializeField] private bool isLooping;
	[SerializeField] private Frame[] frames;
	public float animationSpeedMultiplier = 1;

	private Frame currentFrame;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		currentFrame = frames[0];
	}

	private void Update()
	{
		if (isResumed && currentFrame.IsFinishedPlaying())
		{
			UpdateCurrentFrame();
		}

		currentFrame.IncrementCurrentTimeSpent(Time.deltaTime * animationSpeedMultiplier);
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

	public void Pause() => isResumed = false;

	public void Resume() => isResumed = true;

	public void ResetAnimation() => currentFrame = frames[0];

}
