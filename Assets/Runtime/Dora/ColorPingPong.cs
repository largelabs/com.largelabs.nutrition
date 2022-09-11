using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPingPong : MonoBehaviour
{
    [SerializeField] private SpriteRenderer rnd = null;
    [SerializeField] private Color baseColor = Color.yellow;
    [SerializeField] private Color targetColor = Color.magenta;
    [SerializeField] private InterpolatorsManager interpolatorsManager = null;
    [SerializeField] private AnimationCurve singleLerpCurve = null;

    bool isPaused = false;
    Coroutine pingPongRoutine = null;

    #region PUBLIC API
    public void StartPingPong(float i_singleLerpTime, 
                              Color? i_baseColor, 
                              Color? i_targetColor,
                              int i_numberOfLerps)
    {
        if (pingPongRoutine == null)
            pingPongRoutine = StartCoroutine(pingPongSequence(i_singleLerpTime, i_baseColor, i_targetColor, i_numberOfLerps));
    }

    public void StopPingPong()
    {
        this.DisposeCoroutine(ref pingPongRoutine);
    }

    public void PausePingPong()
    {

    }

    public void ResumePingPong()
    {

    }
    #endregion

    #region PRIVATE 
    private IEnumerator pingPongSequence(float i_singleLerpTime, 
                                         Color? i_baseColor, 
                                         Color? i_targetColor,
                                         int i_numberOfLerps)
    {
        AnimationMode mode = new AnimationMode(singleLerpCurve);

        Color color_0 = i_baseColor != null ? i_baseColor.Value : baseColor;
        Color color_1 = i_targetColor != null ? i_targetColor.Value : targetColor;

        ITypedAnimator<Color> colorInterpolator = interpolatorsManager.Animate(color_0, color_1, i_singleLerpTime, mode, true, 0f, null);

        while (colorInterpolator.IsActive)
        {
            rnd.color = colorInterpolator.Current;
            yield return null;
        }

        this.DisposeCoroutine(ref pingPongRoutine);
    }
    #endregion
}
