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
    [SerializeField] private GameObject charcoalGroup = null;
 
    [Header("Camera")]
    [SerializeField] private PanCamera panCamera = null;
    [SerializeField] private float panDownTime = 0.2f;
    [SerializeField] private float panUpTime = 0.5f;
    [SerializeField] private float exitTime = 0.2f;

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

            enableOffScreenCobKernels(true);

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

    public void ReverseQueue()
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
            yield return StartCoroutine(animateToTransform(currentCob, doneAnchor, exitTime,
                                            playMoveCurve, onMoveToDone));

        }

        panCamera.PanCameraDown(panDownTime);
        yield return this.Wait(panDownTime + 0.2f);

        if (i_nextCob != null)
        {
            DoraDurabilityManager dorabilityManager = i_nextCob.GetComponent<DoraDurabilityManager>();
            if (dorabilityManager != null)
                dorabilityManager.DeactivateDurabilityUpdate();

            panCamera.PanCameraUp(panUpTime);
            yield return StartCoroutine(animateToTransform(i_nextCob, playAnchor, panUpTime,
                                            playMoveCurve, null));

            enableOffScreenCobKernels(false);

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

    private void enableOffScreenCobKernels(bool i_enable)
    {
        DoraCellMap currDora = null;
        foreach (Transform dora in doraCobQueue)
        {
            currDora = dora.GetComponent<DoraCellMap>();

            if (currDora != null)
                currDora.EnableRenderers(i_enable);
        }

        charcoalGroup.SetActive(i_enable);
    }

    IEnumerator animateToTransform(Transform i_nextCob, Transform i_target, float i_time, AnimationCurve i_curve, Action<ITypedAnimator<Vector3>> i_onAnimationEnded)
    {
        AnimationMode mode = new AnimationMode(i_curve);
        ITypedAnimator<Vector3> posInterpolator = interpolatorManager.Animate(i_nextCob.position, i_target.position, i_time, mode, false, 0f, i_onAnimationEnded);
        Vector3 targetScale = new Vector3(i_target.localScale.x / i_nextCob.lossyScale.x,
                                            i_target.localScale.y / i_nextCob.lossyScale.y,
                                            i_target.localScale.z / i_nextCob.lossyScale.z);
        ITypedAnimator<Vector3> scaleInterpolator = interpolatorManager.Animate(i_nextCob.localScale, targetScale, i_time, mode, false, 0f, null);

        while (true == posInterpolator.IsActive)
        {
            i_nextCob.position = posInterpolator.Current;
            i_nextCob.localScale = scaleInterpolator.Current;
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
