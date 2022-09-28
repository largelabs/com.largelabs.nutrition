using UnityEngine;

public class SpriteFrameSwapper : FrameSwapper<SpriteRenderer, Sprite>
{
	protected override void updateRenderedObject() => renderer.sprite = currentFrame.FrameObject;
	protected override void updateRenderedObject(Sprite i_sprite) => renderer.sprite = i_sprite;

}
