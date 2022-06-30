using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FrameSwapper : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;
	private bool isPlaying = false;

	[SerializeField] private Frame[] frames;
	[SerializeField] private bool isLooping;

	private Frame currentFrame;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		currentFrame = frames[0];
	}

	private void Update()
	{
		if (isPlaying)
		{
			UpdateCurrentFrame();
			spriteRenderer.sprite = currentFrame.GetSprite();
		}
	}

	public void Pause() => isPlaying = false;

	public void Resume() => isPlaying = true;

	private void UpdateCurrentFrame()
	{
		if (!isLooping && frames.Last().Equals(currentFrame))
		{
			return;
		}

		currentFrame = CollectionsUtilities.GetNextElementInCircularCollection(currentFrame, frames);
	}

}
