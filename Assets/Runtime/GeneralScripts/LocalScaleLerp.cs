using System.Collections;
using UnityEngine;

public class LocalScaleLerp : MonoBehaviourBase
{
    [SerializeField] private Transform targetTr = null;
    [SerializeField] private Vector2 targetScale = MathConstants.VECTOR_2_ONE;
    [SerializeField] private float animationTimeX = 0.1f;
    [SerializeField] private float animationTimeY = 0.1f;
    [SerializeField] private bool clampX = false;
    [SerializeField] private bool clampY = false;
    [SerializeField] private AnimationCurve xCurve = null;
    [SerializeField] private AnimationCurve yCurve = null;
    [SerializeField] private InterpolatorsManager interpolators = null;
    [SerializeField] private bool resetOnFinish = false;

    ITypedAnimator<float> xAnimator = null;
    ITypedAnimator<float> yAnimator = null;
    Vector3 originalScale = MathConstants.VECTOR_3_ONE;

    Coroutine scaleRoutine = null;

    [ExposePublicMethod]
    public void StartLerp(InterpolatorsManager i_interps)
    {
        interpolators = i_interps;
        if (scaleRoutine == null)
        {
            originalScale = targetTr.localScale;
            scaleRoutine = StartCoroutine(lerpSequence());
        }
    }

    [ExposePublicMethod]
    public void StopLerp()
    {
        this.DisposeCoroutine(ref scaleRoutine);

        if(resetOnFinish)
            targetTr.localScale = originalScale;
    }

    [ExposePublicMethod]
    public void PausePingPong()
    {
        if (xAnimator != null)
            xAnimator.Pause();  
        
        if (yAnimator != null)
            yAnimator.Pause();
    }

    [ExposePublicMethod]
    public void ResumePingPong()
    {
        if (xAnimator != null)
            xAnimator.Resume(); 
        
        if (yAnimator != null)
            yAnimator.Resume();
    }
    
    #region PRIVATE
    private IEnumerator lerpSequence()
    {
        AnimationMode modeX = new AnimationMode(xCurve);
        AnimationMode modeY = new AnimationMode(yCurve);

        xAnimator = interpolators.Animate(targetTr.localScale.x, targetTr.localScale.x * targetScale.x, animationTimeX, modeX, clampX, 0f, null);
        yAnimator = interpolators.Animate(targetTr.localScale.y, targetTr.localScale.y * targetScale.y, animationTimeY, modeY, clampY, 0f, null);

        while (xAnimator.IsActive || yAnimator.IsActive)
        {
            targetTr.localScale = new Vector3(
                xAnimator.IsActive ? xAnimator.Current : targetTr.localScale.x,
                yAnimator.IsActive ? yAnimator.Current : targetTr.localScale.y,
                targetTr.localScale.z);

            yield return null;
        }

        if (resetOnFinish)
            targetTr.localScale = originalScale;
        yield return null;

        this.DisposeCoroutine(ref scaleRoutine);

    }
    #endregion
}
