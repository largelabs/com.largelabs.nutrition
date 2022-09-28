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

    Coroutine scaleRoutine = null;

    public void StartLerp()
    {
        if (scaleRoutine == null)
            scaleRoutine = StartCoroutine(lerpSequence());
    }

    public void StopLerp()
    {
        this.DisposeCoroutine(ref scaleRoutine);
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

        xAnimator = interpolators.Animate(transform.localScale.x, targetScale.x, animationTimeX, modeX, clampX, 0f, null);
        yAnimator = interpolators.Animate(transform.localScale.y, targetScale.y, animationTimeY, modeY, clampY, 0f, null);

        Vector3 original = transform.localScale;

        while (xAnimator.IsActive || yAnimator.IsActive)
        {
            targetTr.localScale = new Vector3(
                xAnimator.Current,
                yAnimator.Current,
                targetTr.localScale.z);

            yield return null;
        }

        if (resetOnFinish)
            targetTr.localScale = original;
        yield return null;

        this.DisposeCoroutine(ref scaleRoutine);

    }
    #endregion
}
