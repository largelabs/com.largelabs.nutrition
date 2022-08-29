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
    [SerializeField] private DoraSpawner doraSpawner = null;

    private Queue<Transform> doraCobQueue = null;
    private Transform currentCob = null;

    Coroutine nextCobRoutine = null;
    public Action<DoraCellMap> OnGetNextCob = null;
    public Action OnQueueEmpty = null;

    #region PUBLIC API
    [ExposePublicMethod]
    public void GetNextCob()
    {
        if (nextCobRoutine == null)
        {
            Transform nextCob = null;

            if (doraCobQueue != null && doraCobQueue.Count > 0)
                nextCob = doraCobQueue.Dequeue();

            nextCobRoutine = StartCoroutine(getNextCob(nextCob));
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

    public void reverseQueue()
    {
        List<Transform> holder = new List<Transform>();
        while (doraCobQueue.Count > 0)
            holder.Add(doraCobQueue.Dequeue());

        int length = holder.Count;
        for (int i = length-1; i >= 0; i--)
        {
            doraCobQueue.Enqueue(holder[i]);
        }
    }
    #endregion

    #region PRIVATE API

    private IEnumerator getNextCob(Transform i_nextCob)
    {
        if (currentCob != null)
        {
            // could possibly do smth different if cob is burnt
            yield return StartCoroutine(animateToPosition(currentCob, doneAnchor.position, 1f,
                                            playMoveCurve, onMoveToDone));
        }

        if (i_nextCob != null)
        {
            DoraDurabilityManager dorabilityManager = i_nextCob.GetComponent<DoraDurabilityManager>();
            if (dorabilityManager != null)
                dorabilityManager.DeactivateDurabilityUpdate();

            yield return StartCoroutine(animateToPosition(i_nextCob, playAnchor.position, 1f,
                                            playMoveCurve, null));

            DoraCellMap cellMap = i_nextCob.GetComponent<DoraCellMap>();
            OnGetNextCob?.Invoke(cellMap);
        }
        else
        {
            OnQueueEmpty?.Invoke();
        }

        currentCob = i_nextCob;          

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

    private void onMoveToDone(ITypedAnimator<Vector3> i_anim)
    {
        if (currentCob == null)
        {
            Debug.LogError("No current cob, how was this triggered?");
            return;
        }

        DoraCellMap cellMap = currentCob.GetComponent<DoraCellMap>();

        if(cellMap != null)
            doraSpawner.DespawnDoraCob(cellMap);
    }
    #endregion
}
