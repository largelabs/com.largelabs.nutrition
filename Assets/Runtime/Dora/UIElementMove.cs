using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIElementMove : MonoBehaviourBase
{
    Coroutine movementRoutine = null;

    #region UNITY AND CORE

    private void OnDisable()
    {
        this.DisposeCoroutine(ref movementRoutine);
    } 
    
    private void OnDestroy()
    {
        this.DisposeCoroutine(ref movementRoutine);
    }

    #endregion

    #region PUBLIC API

    public void MoveToRectTransform(RectTransform i_target, float i_time, 
                                    InterpolatorsManager i_interps, AnimationCurve i_curve,
                                    Action<ITypedAnimator<Vector3>> i_callback)
    {
        MoveToPosition(i_target.position, i_time, i_interps, i_curve, i_callback);
    }

    public void MoveToPosition(Vector3 i_target, float i_time,
                                InterpolatorsManager i_interps, AnimationCurve i_curve,
                                Action<ITypedAnimator<Vector3>> i_callback)
    {
        if (i_interps == null)
        {
            Debug.LogError("Invalid interpolator manager passed! Returning...");
            return;
        }

        if (movementRoutine == null)
        {
            movementRoutine = StartCoroutine(movementSequence(i_target, i_time, i_interps, i_curve, i_callback));
        }
    }

    #endregion

    #region PRIVATE

    private IEnumerator movementSequence(Vector3 i_target, float i_time, 
                                         InterpolatorsManager i_interps, AnimationCurve i_curve, 
                                         Action<ITypedAnimator<Vector3>> i_callback)
    {
        AnimationMode mode = new AnimationMode(i_curve);
        ITypedAnimator<Vector3> posInterpolator = i_interps.Animate(transform.position, i_target, i_time, mode, true, 0f, i_callback);

        while (posInterpolator.IsActive)
        {
            transform.position = posInterpolator.Current;
            yield return null;
        }

        this.DisposeCoroutine(ref movementRoutine);
    }

    #endregion
}
