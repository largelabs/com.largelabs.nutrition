using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UITextColorPingPong : ColorPingPongBase
{
    [SerializeField] private Text thisText = null;

    public override void StartPingPong(float i_singleLerpTime,
                                       Color? i_baseColor,
                                       Color? i_targetColor,
                                       int i_numberOfLerps,
                                       bool i_resetColorOnFinish)
    {
        base.StartPingPong(i_singleLerpTime, i_baseColor, i_targetColor, i_numberOfLerps, i_resetColorOnFinish);
        originalColor = thisText.color;
    }

    [ExposePublicMethod]
    public override void StopPingPong()
    {
        base.StopPingPong();

        if (resetColorOnFinish) 
            thisText.color = originalColor;
    }

    protected override IEnumerator pingPongSequence(float i_singleLerpTime,
                                     Color? i_baseColor,
                                     Color? i_targetColor,
                                     int i_numberOfLerps)
    {
        int remainingLerps = Mathf.Clamp(i_numberOfLerps - 1, -1, int.MaxValue);
        AnimationMode mode = new AnimationMode(singleLerpCurve);

        Color color_0 = i_baseColor != null ? i_baseColor.Value : baseColor;
        Color color_1 = i_targetColor != null ? i_targetColor.Value : targetColor;

        colorInterpolator = interpolatorsManager.Animate(color_0, color_1, i_singleLerpTime, mode, true, 0f, null);

        while (colorInterpolator.IsActive)
        {
            thisText.color = colorInterpolator.Current;
            yield return null;
        }

        this.DisposeCoroutine(ref pingPongRoutine);

        if (remainingLerps != 0)
            pingPongRoutine = StartCoroutine(pingPongSequence(i_singleLerpTime, color_1, color_0, remainingLerps));
        else if (resetColorOnFinish)
            thisText.color = originalColor;
    }
}
