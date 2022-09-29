using UnityEngine;
using UnityEngine.UI;

public class UIImageFrameSwapper : FrameSwapper<Image, Sprite>
{
    protected override void updateRenderedObject() => renderer.sprite = currentFrame.FrameObject;
    protected override void updateRenderedObject(Sprite i_sprite) => renderer.sprite = currentFrame.FrameObject;

}

