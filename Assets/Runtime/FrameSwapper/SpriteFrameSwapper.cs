using UnityEngine;

public class SpriteFrameSwapper : FrameSwapper<SpriteRenderer, Sprite>
{
	protected override void updateRenderedObject() => renderer.sprite = currentFrame.FrameObject;
}
