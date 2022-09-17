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
    [SerializeField] private DoraSFXProvider sfxProvider = null;

    [Header("Camera")]
    [SerializeField] private PanCamera panCamera = null;
    [SerializeField] private float panDownTime = 0.2f;
    [SerializeField] private float panUpTime = 0.5f;
    [SerializeField] private float exitTime = 0.2f;

    private Stack<Transform> doraCobStack = null;
    private Transform currentCob = null;

    Coroutine nextCobRoutine = null;
    public Action<DoraCellMap, AutoRotator> OnGetNextCob = null;
    public Action OnQueueEmpty = null;

    #region PUBLIC API
    [ExposePublicMethod]
    public void GetNextCob()
    {
        if (nextCobRoutine == null)
        {
            Transform nextCob = null;

            enableOffScreenCobKernels(true);

            if (doraCobStack != null && doraCobStack.Count > 0)
                nextCob = doraCobStack.Pop();

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

        if (doraCobStack == null)
            doraCobStack = new Stack<Transform>();

        doraCobStack.Push(i_doraCob);
    }
    #endregion

    #region PRIVATE API

    private IEnumerator getNextCob(Transform i_nextCob)
    {
        if (currentCob != null)
        {
            Debug.LogError("get next cob ");
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

            i_nextCob.transform.position = new Vector3(playAnchor.position.x, playAnchor.position.y, 0);

            enableOffScreenCobKernels(false);

            DoraCellMap cellMap = i_nextCob.GetComponent<DoraCellMap>();
            AutoRotator rotator = i_nextCob.GetComponent<AutoRotator>();
            OnGetNextCob?.Invoke(cellMap, rotator);
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
        foreach (Transform dora in doraCobStack)
        {
            currDora = dora.GetComponent<DoraCellMap>();

            if (currDora != null)
                currDora.EnableRenderers(i_enable);
        }

        charcoalGroup.SetActive(i_enable);
    }
    private int counter = 0;
    IEnumerator animateToTransform(Transform i_nextCob, Transform i_target, float i_time, AnimationCurve i_curve, Action<ITypedAnimator<Vector3>> i_onAnimationEnded)
    {
        counter++;
        Debug.LogError("start the animation<<<<<<<<<<<<<<<<< " +counter);
        AnimationMode mode = new AnimationMode(i_curve);
        ITypedAnimator<Vector3> posInterpolator = interpolatorManager.Animate(i_nextCob.position, i_target.position, i_time, mode, false, 0f, i_onAnimationEnded);
        Vector3 targetScale = new Vector3(i_target.localScale.x / i_nextCob.lossyScale.x,
                                            i_target.localScale.y / i_nextCob.lossyScale.y,
                                            i_target.localScale.z / i_nextCob.lossyScale.z);
        ITypedAnimator<Vector3> scaleInterpolator = interpolatorManager.Animate(i_nextCob.localScale, targetScale, i_time, mode, false, 0f, null);

        sfxProvider.PlayMovementSFX();

        while (true == posInterpolator.IsActive)
        {
            i_nextCob.position = posInterpolator.Current;
            i_nextCob.localScale = scaleInterpolator.Current;
            yield return null;
        }
        Debug.LogError("end the animation<<<<<<<<<<<<<<<<< " + counter);

        //The callback is not called for some reason
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
