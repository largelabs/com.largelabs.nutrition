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
    private Transform currentCob = null;

    Coroutine nextCobRoutine = null;

    #region PUBLIC API
    [ExposePublicMethod]
    public void GetNextCob()
    {
        if (nextCobRoutine == null)
        {
            nextCobRoutine = StartCoroutine(getNextCob());
        }
    }

    public void RegisterCob(Transform i_doraCob)
    {
        if (i_doraCob == null)
        {
            Debug.LogError("Transform is null! Cannot register cob");
            return;
        }

        if (doraCobQueue == null)
            doraCobQueue = new Queue<Transform>();

        doraCobQueue.Enqueue(i_doraCob);
    }

    // implement api to be able to move cobs 
    // grill -> play point
    // play point -> off screen
    // only move to play point if there is a cob on the grill and no cob on the play point
    #endregion

    #region PRIVATE API

    private IEnumerator getNextCob()
    {
        if (currentCob != null)
        {
            yield return StartCoroutine(animateToPosition(currentCob, doneAnchor.position, 1f,
                                            playMoveCurve, onPlayMoveDone));
        }

        if (doraCobQueue != null && doraCobQueue.Count > 0)
        {
            Transform nextCob = doraCobQueue.Dequeue();
            if (nextCob != null)
            {
                DoraDurabilityManager dorabilityManager = nextCob.GetComponent<DoraDurabilityManager>();
                if (dorabilityManager != null)
                    dorabilityManager.DeactivateDurabilityUpdate();

                yield return StartCoroutine(animateToPosition(nextCob, playAnchor.position, 1f,
                                                playMoveCurve, onPlayMoveDone));
            }

            currentCob = nextCob;
        }
        else
            currentCob = null;

        this.DisposeCoroutine(ref nextCobRoutine);
    }

    IEnumerator animateToPosition(Transform i_nextCob, Vector3 i_target, float i_time, AnimationCurve i_curve, Action<ITypedAnimator<Vector3>> i_onAnimationEnded)
    {
        AnimationMode mode = new AnimationMode(i_curve);
        ITypedAnimator<Vector3> posInterpolator = interpolatorManager.Animate(i_nextCob.position, i_target, i_time, mode, false, 0f, i_onAnimationEnded);

        while (true == posInterpolator.IsActive)
        {
            i_nextCob.position = posInterpolator.Current;
            yield return null;
        }
    }

    private void onPlayMoveDone(ITypedAnimator<Vector3> i_anim)
    {
    }
    #endregion
}
