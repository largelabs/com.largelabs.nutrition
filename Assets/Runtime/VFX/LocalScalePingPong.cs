using System.Collections;
using UnityEngine;

public class LocalScalePingPong : MonoBehaviourBase
{
    [SerializeField] private Transform tr = null;
    [SerializeField] private Vector3 baseScale = Vector3.one;
    [SerializeField] private Vector3 targetScale = Vector3.one;
    [SerializeField] private InterpolatorsManager interpolatorsManager = null;
    [SerializeField] private AnimationCurve singleLerpCurve = null;
    [SerializeField] private bool clampValues = true;
    [SerializeField] private bool resetScaleOnFinish = true;

    private ITypedAnimator<Vector3> scaleInterpolator = null;
    private Coroutine pingPongRoutine = null;
    private Vector3 originalScale = Vector3.one;

    private void OnDestroy()
    {
        StopPingPong();
    }

    #region PUBLIC API
    public bool isScaling => pingPongRoutine != null;

    public void StartPingPong(float i_singleLerpTime,
                              Vector3? i_baseScale,
                              Vector3? i_targetScale,
                              int i_numberOfLerps,
                              bool i_resetColorOnFinish)
    {
        resetScaleOnFinish = i_resetColorOnFinish;
        originalScale = tr.localScale;
        if (pingPongRoutine == null)
            pingPongRoutine = StartCoroutine(pingPongSequence(i_singleLerpTime, i_baseScale, i_targetScale, i_numberOfLerps));
    }

    [ExposePublicMethod]
    public void StartPingPong(float i_singleLerpTime,
                              int i_numberOfLerps)
    {
        if (pingPongRoutine == null)
            pingPongRoutine = StartCoroutine(pingPongSequence(i_singleLerpTime, null, null, i_numberOfLerps));
    }

    [ExposePublicMethod]
    public void StopPingPong()
    {
        this.DisposeCoroutine(ref pingPongRoutine);

        if (resetScaleOnFinish)
            tr.localScale = originalScale;
    }

    [ExposePublicMethod]
    public void PausePingPong()
    {
        if (scaleInterpolator != null)
            scaleInterpolator.Pause();
    }

    [ExposePublicMethod]
    public void ResumePingPong()
    {
        if (scaleInterpolator != null)
            scaleInterpolator.Resume();
    }

    public void SetScale(Vector3 i_scale)
    {
        tr.localScale = i_scale;
    }

    public void AssignInterpolators(InterpolatorsManager i_interps)
    {
        interpolatorsManager = i_interps;
    }
    #endregion

    #region PRIVATE 
    private IEnumerator pingPongSequence(float i_singleLerpTime,
                                         Vector3? i_baseScale,
                                         Vector3? i_targetScale,
                                         int i_numberOfLerps)
    {
        int remainingLerps = Mathf.Clamp(i_numberOfLerps - 1, -1, int.MaxValue);
        AnimationMode mode = new AnimationMode(singleLerpCurve);

        Vector3 scale_0 = i_baseScale != null ? i_baseScale.Value : baseScale;
        Vector3 scale_1 = i_targetScale != null ? i_targetScale.Value : targetScale;

        scaleInterpolator = interpolatorsManager.Animate(scale_0, scale_1, i_singleLerpTime, mode, clampValues, 0f, null);

        while (scaleInterpolator.IsActive)
        {
            tr.localScale = scaleInterpolator.Current;
            yield return null;
        }

        this.DisposeCoroutine(ref pingPongRoutine);

        if (remainingLerps != 0)
            pingPongRoutine = StartCoroutine(pingPongSequence(i_singleLerpTime, scale_1, scale_0, remainingLerps));
        else if (resetScaleOnFinish)
            tr.localScale = originalScale;
    }
    #endregion
}
