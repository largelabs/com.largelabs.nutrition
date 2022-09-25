using System;
using System.Collections;
using UnityEngine;

public class UIAppearWithCanvasGroupAlpha : MonoBehaviourBase, IAppear
{
    [SerializeField] CanvasGroup animatedCanvasGroup = null;
    [SerializeField] AnimationCurve appearCurve = null;
    [SerializeField] AnimationCurve disappearCurve = null;
    [SerializeField] float appearTime = 0.5f;
    [SerializeField] float disappearTime = 0.5f;
    [SerializeField] bool deactivateOnDisappear = true;


    InterpolatorsManager interpolators = null;
    Coroutine appearRoutine = null;
    Coroutine disappearRoutine = null;
    bool isInit = false;

    public void Init(InterpolatorsManager i_interpolators)
    {
        if (true == isInit) return;

        animatedCanvasGroup.gameObject.SetActive(false);
        interpolators = i_interpolators;
        isInit = true;
    }


    #region IAppear

    public bool IsAppearInit => isInit;

    [ExposePublicMethod]
    public void Appear(bool i_animated)
    {
        animatedCanvasGroup.gameObject.SetActive(true);

        if (true == i_animated)
        {
            if (null == appearRoutine) appearRoutine = StartCoroutine(animateAlpha(0f, 1f, appearTime, appearCurve, onDidAppear));
        }
        else
        {
            onDidAppear(null);
        }
    }

    [ExposePublicMethod]
    public void Disappear(bool i_animated)
    {
        animatedCanvasGroup.gameObject.SetActive(true);

        if (true == i_animated)
        {
            if (null == disappearRoutine) disappearRoutine = StartCoroutine(animateAlpha(animatedCanvasGroup.alpha, 0f, disappearTime, disappearCurve, onDidDisappear));
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

    IEnumerator animateAlpha(float i_start, float i_target, float i_time, AnimationCurve i_curve, Action<ITypedAnimator<float>> i_onAnimationEnded)
    {
        animatedCanvasGroup.alpha = i_start;

        AnimationMode mode = new AnimationMode(i_curve);
        ITypedAnimator<float> scaleInterpolator = interpolators.Animate(i_start, i_target, i_time, mode, false, 0f, i_onAnimationEnded);

        while (true == scaleInterpolator.IsActive)
        {
            animatedCanvasGroup.alpha = scaleInterpolator.Current;
            yield return null;
        }
    }

    void onDidAppear(ITypedAnimator<float> i_interpolator)
    {
        animatedCanvasGroup.alpha = 1f;
        this.DisposeCoroutine(ref appearRoutine);
    }

    void onDidDisappear(ITypedAnimator<float> i_interpolator)
    {
        animatedCanvasGroup.alpha = 0f;
        this.DisposeCoroutine(ref disappearRoutine);

        if (true == deactivateOnDisappear) animatedCanvasGroup.gameObject.SetActive(false);
    }

    #endregion
}


