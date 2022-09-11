using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPingPong : MonoBehaviourBase
{
    [SerializeField] private SpriteRenderer rnd = null;
    [SerializeField] protected Color baseColor = Color.yellow;
    [SerializeField] protected Color targetColor = Color.magenta;
    [SerializeField] protected InterpolatorsManager interpolatorsManager = null;
    [SerializeField] protected AnimationCurve singleLerpCurve = null;

    protected ITypedAnimator<Color> colorInterpolator = null;
    protected Coroutine pingPongRoutine = null;
    protected Color originalColor = Color.white;
    protected bool resetColorOnFinish = true;

    private void OnDestroy()
    {
        StopPingPong();
    }

    #region PUBLIC API
    public virtual void StartPingPong(float i_singleLerpTime, 
                                      Color? i_baseColor, 
                                      Color? i_targetColor,
                                      int i_numberOfLerps,
                                      bool i_resetColorOnFinish)
    {
        resetColorOnFinish = i_resetColorOnFinish;
        originalColor = rnd.color;
        if (pingPongRoutine == null)
            pingPongRoutine = StartCoroutine(pingPongSequence(i_singleLerpTime, i_baseColor, i_targetColor, i_numberOfLerps));
    }
    
    [ExposePublicMethod]
    public void StartPingPong(float i_singleLerpTime,
                              int i_numberOfLerps)
    {
        if (pingPongRoutine == null)
            pingPongRoutine = StartCoroutine(pingPongSequence(i_singleLerpTime, null, null, i_numberOfLerps));
    }

    [ExposePublicMethod]
    public virtual void StopPingPong()
    {
        this.DisposeCoroutine(ref pingPongRoutine);

        if(resetColorOnFinish)
            rnd.color = originalColor;
    }

    [ExposePublicMethod]
    public void PausePingPong()
    {
        if (colorInterpolator != null)
            colorInterpolator.Pause();
    }

    [ExposePublicMethod]
    public void ResumePingPong()
    {
        if (colorInterpolator != null)
            colorInterpolator.Resume();
    }
    #endregion

    #region PRIVATE 
    protected virtual IEnumerator pingPongSequence(float i_singleLerpTime, 
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
            rnd.color = colorInterpolator.Current;
            yield return null;
        }

        this.DisposeCoroutine(ref pingPongRoutine);

        if (remainingLerps != 0)
            pingPongRoutine = StartCoroutine(pingPongSequence(i_singleLerpTime, color_1, color_0, remainingLerps));
        else if(resetColorOnFinish)
            rnd.color = originalColor;
    }
    #endregion
}
