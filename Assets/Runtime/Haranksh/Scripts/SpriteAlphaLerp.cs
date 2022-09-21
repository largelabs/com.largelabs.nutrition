using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpriteAlphaLerp : MonoBehaviourBase
{
    [SerializeField] private SpriteRenderer thisRenderer = null;
    [SerializeField] float initialAlpha = 0.0f;
    [SerializeField] float targetAlpha = 1.0f;
    [SerializeField] float animationTime = 0.5f;
    [SerializeField] InterpolatorsManager interpolatorsManager = null;
    [SerializeField] AnimationCurve animationCurve = null;
    [SerializeField] bool clampValues = true;

    Coroutine alphaLerpRoutine = null;

    float initialAlphaLocal = 0.0f;
    float targetAlphaLocal = 1.0f;
    float animationTimeLocal = 0.5f;
    InterpolatorsManager interpolatorsManagerLocal = null;
    AnimationCurve animationCurveLocal = null;
    bool clampValuesLocal = true;

    #region UNITY
    protected override void Awake()
    {
        base.Awake();

        getRenderer();
    }

    private void OnDisable()
    {
        this.DisposeCoroutine(ref alphaLerpRoutine);
    }

    private void OnDestroy()
    {
        this.DisposeCoroutine(ref alphaLerpRoutine);
    }

    #endregion

    #region PUBLIC API
    public bool IsAnimated => null != alphaLerpRoutine;

    public void LerpAlpha(float? i_initial, float? i_target, float? i_time,
                            InterpolatorsManager i_interps, AnimationCurve i_curve, bool? i_clamp,
                            Action<ITypedAnimator<float>> i_callback)
    {
        if (i_interps == null)
        {
            Debug.LogError("Invalid interpolator manager passed! Returning...");
            return;
        }

        setInterpolatorValues(i_initial, i_target, i_time, i_interps, i_curve, i_clamp);

        if (alphaLerpRoutine == null)
            alphaLerpRoutine = StartCoroutine(alphaLerpSequence(i_callback));
    }
    #endregion

    #region PRIVATE
    private bool getRenderer()
    {
        if (thisRenderer == null)
            thisRenderer = GetComponent<SpriteRenderer>();

        if (thisRenderer == null)
        {
            Debug.LogError("No renderer assigned!");
            return false;
        }

        return true;
    }

    private void setInterpolatorValues(
        float? i_initial, 
        float? i_target, 
        float? i_time, 
        InterpolatorsManager i_interps, 
        AnimationCurve i_curve, 
        bool? i_clamp)
    {
        initialAlphaLocal = i_initial == null? initialAlpha:i_initial.Value;
        targetAlphaLocal = i_target == null? targetAlpha:i_target.Value;
        animationTimeLocal = i_time == null? animationTime:i_time.Value;
        interpolatorsManagerLocal = i_interps == null? interpolatorsManager:i_interps;
        animationCurveLocal = i_curve == null? animationCurve:i_curve;
        clampValuesLocal = i_clamp == null? clampValues:i_clamp.Value;
    }

    private IEnumerator alphaLerpSequence(Action<ITypedAnimator<float>> i_callback)
    {
        if(getRenderer() == false)
            yield break;

        AnimationMode mode = new AnimationMode(animationCurveLocal);
        ITypedAnimator<float> alphaInterpolator = 
            interpolatorsManagerLocal.Animate(initialAlphaLocal, targetAlphaLocal, animationTimeLocal, 
            mode, clampValuesLocal, 0f, i_callback);

        while (alphaInterpolator.IsActive)
        {
            updateImageAlpha(alphaInterpolator.Current);
            yield return null;
        }

        this.DisposeCoroutine(ref alphaLerpRoutine);
    }

    private void updateImageAlpha(float i_alpha)
    {
        if (getRenderer() == false)
            return;

        Color temp = thisRenderer.color;
        thisRenderer.color = new Color(temp.r, temp.g, temp.b, i_alpha);
    }
    #endregion
}
