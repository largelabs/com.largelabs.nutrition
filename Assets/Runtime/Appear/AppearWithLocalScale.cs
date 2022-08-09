using System;
using System.Collections;
using UnityEngine;

public class AppearWithLocalScale : MonoBehaviourBase, IAppear
{
    [SerializeField] AnimationCurve appearCurve = null;
    [SerializeField] AnimationCurve disappearCurve = null;

    InterpolatorsManager interpolators = null;
    Coroutine appearRoutine = null;
    Coroutine disappearRoutine = null;
    bool isInit = false;

    public void Init(InterpolatorsManager i_interpolators)
    {
        if (true == isInit) return;

        gameObject.SetActive(false);
        interpolators = i_interpolators;
        isInit = true;
    }

    #region IAppear

    [ExposePublicMethod]
    public void Appear(bool i_animated)
    {
        gameObject.SetActive(true);

        if (true == i_animated)
        {
            if (null == appearRoutine) appearRoutine = StartCoroutine(animateLocalScale(MathConstants.VECTOR_3_ZERO, MathConstants.VECTOR_3_ONE, 0.5f, appearCurve, onDidAppear));
        }
        else
        {
            onDidAppear(null);
        }
    }

    [ExposePublicMethod]
    public void Disappear(bool i_animated)
    {
        gameObject.SetActive(true);

        if (true == i_animated)
        {
            if (null == disappearRoutine) disappearRoutine = StartCoroutine(animateLocalScale(transform.localScale, MathConstants.VECTOR_3_ZERO, 0.2f, disappearCurve, onDidDisappear));
        }
        else
        {
            onDidDisappear(null);
        }
    }

    public bool IsAppearing => null != appearRoutine;

    public bool IsDisappearing => null != disappearRoutine;

    #endregion

    #region PRIVATE

    IEnumerator animateLocalScale(Vector3 i_start, Vector3 i_target, float i_time, AnimationCurve i_curve, Action<ITypedAnimator<Vector3>> i_onAnimationEnded)
    {
        transform.localScale = i_start;

        AnimationMode mode = new AnimationMode(i_curve);
        ITypedAnimator<Vector3> scaleInterpolator = interpolators.Animate(i_start, i_target, i_time, mode, false, 0f, i_onAnimationEnded);

        while (true == scaleInterpolator.IsActive)
        {
            transform.localScale = scaleInterpolator.Current;
            yield return null;
        }
    }

    void onDidAppear(ITypedAnimator<Vector3> i_interpolator)
    {
        transform.localScale = MathConstants.VECTOR_3_ONE;
        this.DisposeCoroutine(ref appearRoutine);
    }

    void onDidDisappear(ITypedAnimator<Vector3> i_interpolator)
    {
        transform.localScale = MathConstants.VECTOR_3_ZERO;
        this.DisposeCoroutine(ref disappearRoutine);
    }

    #endregion
}
