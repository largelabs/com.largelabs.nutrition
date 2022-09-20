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
    public Action<DoraCellMap, AutoRotator, DoraDurabilityManager> OnGetNextCob = null;
    public Action OnQueueEmpty = null;

    #region PUBLIC API
    [ExposePublicMethod]
    public void GetNextCob()
    {
        if (null != nextCobRoutine) return;

        enableOffScreenCobKernels(true);

        Transform nextCob = null;
        if (doraCobStack != null && doraCobStack.Count > 0)
        {
            nextCob = doraCobStack.Pop();
            nextCobRoutine = StartCoroutine(getNextCob(nextCob));
        }
        else
        {
            OnQueueEmpty?.Invoke();
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

    private IEnumerator moveCobOffScreen(Transform i_cob)
    {
        if (null == i_cob) yield break;

        yield return StartCoroutine(animateToTransform(i_cob, doneAnchor, exitTime, playMoveCurve));

        onMoveToDone(i_cob);
    }

    private IEnumerator getNextCob(Transform i_nextCob)
    {
        yield return StartCoroutine(moveCobOffScreen(currentCob));

        currentCob = i_nextCob;
        DoraDurabilityManager durability = currentCob.GetComponent<DoraDurabilityManager>();
        durability.UpdateDurability(true);

        panCamera.PanCameraDown(panDownTime);
        yield return this.Wait(panDownTime + 0.2f);

        panCamera.PanCameraUp(panUpTime);
        yield return StartCoroutine(animateToTransform(currentCob, playAnchor, panUpTime,
                                        playMoveCurve));

        currentCob.transform.position = new Vector3(playAnchor.position.x, playAnchor.position.y, 0);

        enableOffScreenCobKernels(false);

        DoraCellMap cellMap = currentCob.GetComponent<DoraCellMap>();
        AutoRotator rotator = currentCob.GetComponent<AutoRotator>();

        OnGetNextCob?.Invoke(cellMap, rotator, durability);

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
    IEnumerator animateToTransform(Transform i_nextCob, Transform i_target, float i_time, AnimationCurve i_curve)
    {
        counter++;
        Debug.LogError("start the animation<<<<<<<<<<<<<<<<< " +counter);
        AnimationMode mode = new AnimationMode(i_curve);

        Vector3 targetScale = new Vector3(i_target.localScale.x / i_nextCob.lossyScale.x,
                                            i_target.localScale.y / i_nextCob.lossyScale.y,
                                            i_target.localScale.z / i_nextCob.lossyScale.z);
        ITypedAnimator<Vector3> scaleInterpolator = interpolatorManager.Animate(i_nextCob.localScale, targetScale, i_time, mode, false, 0f, null);
        ITypedAnimator<Vector3> posInterpolator = interpolatorManager.Animate(i_nextCob.position, i_target.position, i_time, mode, false, 0f, null);

        sfxProvider.PlayMovementSFX();

        while (true == posInterpolator.IsActive)
        {
            i_nextCob.position = posInterpolator.Current;
            i_nextCob.localScale = scaleInterpolator.Current;
            yield return null;
        }

        Debug.LogError("end the animation<<<<<<<<<<<<<<<<< " + counter);
    }

    private void onMoveToDone(Transform i_cob)
    {
        if (null == i_cob) return;

        DoraCellMap cellMap = i_cob.GetComponent<DoraCellMap>();
        if(cellMap != null) doraSpawner.DespawnDoraCob(cellMap);
    }

    #endregion
}
