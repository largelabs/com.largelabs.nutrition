using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationType
{
    Linear,
    Ease_In_Out,
    Bounce,
    Custom
}
public struct AnimationMode
{
    AnimationType type;
    AnimationCurve curve;
    public AnimationMode(AnimationType t)
    {
        if (t == AnimationType.Linear)
        {
            type = AnimationType.Linear;
            curve = AnimationCurve.Linear(0, 0, 1, 1);
        }
        else if (t == AnimationType.Ease_In_Out)
        {
            type = AnimationType.Ease_In_Out;
            curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }
        else if (t == AnimationType.Bounce)
        {
            type = AnimationType.Bounce;
            curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        }
        else
        {
            type = AnimationType.Custom;
            curve = AnimationCurve.Linear(0,0,1,1);
            Debug.LogException(new System.Exception("ERROR, Animation Curve Not Specified"));
        }
    }

    public AnimationMode(AnimationCurve c)
    {
        curve = c;
        type = AnimationType.Custom;
    }
    public AnimationType Type => type;
    public AnimationCurve Curve => curve;
}