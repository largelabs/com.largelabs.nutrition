using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoraMover : MonoBehaviourBase
{
    [SerializeField] private Transform playAnchor = null;
    [SerializeField] private Transform doneAnchor = null;
    [SerializeField] private InterpolatorsManager interpolatorManager = null;
    [SerializeField] private AnimationCurve playMoveCurve = null;
    [SerializeField] private AnimationCurve doneMoveCurve = null;

    private Queue<Transform> doraCobQueue = null;

    Coroutine nextCobRoutine = null;

    #region PUBLIC API
    [ExposePublicMethod]
    public void GetNextCob()
    {
        if (doraCobQueue == null || doraCobQueue.Count < 1) return;

        if (nextCobRoutine == null)
        {
            Transform doraCob = doraCobQueue.Dequeue();

            nextCobRoutine = 
                StartCoroutine(animateToPosition(doraCob, playAnchor.localPosition, 1f, 
                                                    playMoveCurve, onPlayMoveDone));
        }
    }

    public void RegisterCob(Transform i_doraCob)
    {
        doraCobQueue.Enqueue(i_doraCob);
    }

    // implement api to be able to move cobs 
    // grill -> play point
    // play point -> off screen
    // only move to play point if there is a cob on the grill and no cob on the play point
    #endregion

    #region PRIVATE

    IEnumerator animateToPosition(Transform i_nextCob, Vector3 i_target, float i_time, AnimationCurve i_curve, Action<ITypedAnimator<Vector3>> i_onAnimationEnded)
    {
        AnimationMode mode = new AnimationMode(i_curve);
        ITypedAnimator<Vector3> posInterpolator = interpolatorManager.Animate(i_nextCob.localPosition, i_target, i_time, mode, false, 0f, i_onAnimationEnded);

        while (true == posInterpolator.IsActive)
        {
            i_nextCob.localPosition = posInterpolator.Current;
            yield return null;
        }
    }

    private void onPlayMoveDone(ITypedAnimator<Vector3> i_anim)
    {
        this.DisposeCoroutine(ref nextCobRoutine);
    }
    #endregion
}
