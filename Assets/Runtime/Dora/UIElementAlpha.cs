using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
public class UIElementAlpha : MonoBehaviourBase
{
    Image thisImage = null;
    Coroutine alphaLerpRoutine = null;

    public bool IsAnimated => null != alphaLerpRoutine;

    private void OnDisable()
    {
        this.DisposeCoroutine(ref alphaLerpRoutine);
    }

    private void OnDestroy()
    {
        this.DisposeCoroutine(ref alphaLerpRoutine);
    }

    public void lerpAlpha(float i_initial, float i_target, float i_time,
                            InterpolatorsManager i_interps, AnimationCurve i_curve,
                            Action<ITypedAnimator<float>> i_callback)
    {
        if (i_interps == null)
        {
            Debug.LogError("Invalid interpolator manager passed! Returning...");
            return;
        }

        if (alphaLerpRoutine == null)
            alphaLerpRoutine = StartCoroutine(alphaLerpSequence(i_initial, i_target, i_time, i_interps, i_curve, i_callback));
    }

    private IEnumerator alphaLerpSequence(float i_initial, float i_target, float i_time, 
                                          InterpolatorsManager i_interps, AnimationCurve i_curve,
                                          Action<ITypedAnimator<float>> i_callback)
    {
        if (thisImage == null)
            thisImage = GetComponent<Image>();

        if (thisImage == null)
        {
            Debug.LogError("No image component found! Required component!");
            yield break;
        }

        AnimationMode mode = new AnimationMode(i_curve);
        ITypedAnimator<float> alphaInterpolator = i_interps.Animate(i_initial, i_target, i_time, mode, true, 0f, i_callback);

        while (alphaInterpolator.IsActive)
        {
            updateImageAlpha(alphaInterpolator.Current);
            yield return null;
        }

        this.DisposeCoroutine(ref alphaLerpRoutine);
    }

    private void updateImageAlpha(float i_alpha)
    {
        if (thisImage == null)
            return;

        Color temp = thisImage.color;
        thisImage.color = new Color(temp.r, temp.g, temp.b, i_alpha);
    }
}
