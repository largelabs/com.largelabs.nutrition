using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIElementMove : MonoBehaviourBase
{
    Coroutine movementRoutine = null;
    
    public void moveToRectTransform(RectTransform i_target, float i_time, 
                                    InterpolatorsManager i_interps, AnimationCurve i_curve,
                                    Action<ITypedAnimator<Vector3>> i_callback)
    {
        if (i_interps == null)
        {
            Debug.LogError("Invalid interpolator manager passed! Returning...");
            return;
        }

        if (movementRoutine == null)
            movementRoutine = StartCoroutine(movementSequence(i_target, i_time, i_interps, i_curve, i_callback));
    }

    private IEnumerator movementSequence(RectTransform i_target, float i_time, 
                                         InterpolatorsManager i_interps, AnimationCurve i_curve, 
                                         Action<ITypedAnimator<Vector3>> i_callback)
    {
        AnimationMode mode = new AnimationMode(i_curve);
        ITypedAnimator<Vector3> posInterpolator = i_interps.Animate(this.transform.position, i_target.position, i_time, mode, true, 0f, i_callback);

        while (posInterpolator.IsActive)
        {
            this.transform.position = posInterpolator.Current;
            yield return null;
        }

        this.DisposeCoroutine(ref movementRoutine);
    }
}
