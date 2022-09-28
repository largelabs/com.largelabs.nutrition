using System.Collections;
using UnityEngine;

public class LocalPositionPingPong : MonoBehaviourBase
{
    [Header("Base Configs")]
    [SerializeField] private Transform tr = null;
    [SerializeField] private Vector3 baseLocalPos = Vector3.zero;
    [SerializeField] private Vector3 targetLocalPos = Vector3.zero;
    [SerializeField] private InterpolatorsManager interpolatorsManager = null;
    [SerializeField] private bool moveX = true;
    [SerializeField] private bool moveY = true;
    [SerializeField] private bool moveZ = true;

    [Header("Animation Configs")]
    [SerializeField] private float singleLerpDurationMax = 0.1f;
    [SerializeField] private float singleLerpDurationMin = 0.1f;
    [SerializeField] private AnimationCurve singleLerpDurationVarianceCurve = null;
    [SerializeField] private AnimationCurve singleLerpCurve = null;
    [SerializeField] private int totalLerps = 1;
    [SerializeField] private bool clampValues = true;

    [Header("Extra Configs")]
    [SerializeField] private bool resetOnFinish = true;
    [SerializeField] private bool flipTargetXAfterEachLerp = false;
    [SerializeField] private bool flipTargetYAfterEachLerp = false;

    private ITypedAnimator<Vector3> localPosInterpolator = null;
    private Coroutine pingPongRoutine = null;
    private Vector3 originalLocalPos = Vector3.one;

    private int lerpsDone = 0;

    private void OnDestroy()
    {
        StopPingPong();
    }

    #region PUBLIC API
    public bool isMoving => pingPongRoutine != null;

    public void StartPingPong(float? i_singleLerpTimeMax = null,
                              float? i_singleLerpTimeMin = null,
                              Vector3? i_baseLocalPos = null,
                              Vector3? i_targetLocalPos = null,
                              int? i_numberOfLerps = null,
                              bool? i_resetOnFinish = null)
    {
        resetOnFinish = i_resetOnFinish == null? resetOnFinish:i_resetOnFinish.Value;
        if (pingPongRoutine == null)
        {
            originalLocalPos = tr.localPosition;
            lerpsDone = 0;
            pingPongRoutine = StartCoroutine(
                pingPongSequence(i_singleLerpTimeMax == null? singleLerpDurationMax: i_singleLerpTimeMax.Value,
                i_singleLerpTimeMin == null? singleLerpDurationMin: i_singleLerpTimeMin.Value, 
                i_baseLocalPos, 
                i_targetLocalPos, 
                i_numberOfLerps == null? totalLerps:i_numberOfLerps.Value));
        }
    }
    
    [ExposePublicMethod]
    public void StartPingPong()
    {
        StartPingPong(null, null, null, null, null);
    }

    [ExposePublicMethod]
    public void StopPingPong()
    {
        this.DisposeCoroutine(ref pingPongRoutine);

        if (resetOnFinish)
            tr.localPosition = new Vector3(
                originalLocalPos.x,
                originalLocalPos.y,
                originalLocalPos.z);
    }

    [ExposePublicMethod]
    public void PausePingPong()
    {
        if (localPosInterpolator != null)
            localPosInterpolator.Pause();
    }

    [ExposePublicMethod]
    public void ResumePingPong()
    {
        if (localPosInterpolator != null)
            localPosInterpolator.Resume();
    }

    public void SetLocalPos(Vector3 i_localPos)
    {
        tr.localPosition = i_localPos;
    }

    public void AssignInterpolators(InterpolatorsManager i_interps)
    {
        interpolatorsManager = i_interps;
    }
    #endregion

    #region PRIVATE 
    private IEnumerator pingPongSequence(float i_singleLerpTimeMax,
                                         float i_singleLerpTimeMin,
                                         Vector3? i_baseLocalPos,
                                         Vector3? i_targetLocalPos,
                                         int i_numberOfLerps)
    {
        int remainingLerps = Mathf.Clamp(i_numberOfLerps - 1, -1, int.MaxValue);
        AnimationMode mode = new AnimationMode(singleLerpCurve);

        Vector3 localPos_0 = i_baseLocalPos != null ? i_baseLocalPos.Value : baseLocalPos;
        Vector3 localPos_1 = i_targetLocalPos != null ? i_targetLocalPos.Value : targetLocalPos;

        float lerpTime = Mathf.Lerp(i_singleLerpTimeMin, i_singleLerpTimeMax,
            singleLerpDurationVarianceCurve.Evaluate(lerpsDone / (float)totalLerps));

        localPosInterpolator = interpolatorsManager.Animate(localPos_0, localPos_1, lerpTime, mode, clampValues, 0f, null);

        while (localPosInterpolator.IsActive)
        {
            tr.localPosition = new Vector3(
                localPosInterpolator.Current.x,
                localPosInterpolator.Current.y,
                localPosInterpolator.Current.z);

            yield return null;
        }

        this.DisposeCoroutine(ref pingPongRoutine);
        lerpsDone++;

        if (remainingLerps != 0)
        {
            localPos_0 = new Vector3(
                flipTargetXAfterEachLerp ? -localPos_0.x : localPos_0.x,
                flipTargetYAfterEachLerp ? -localPos_0.y : localPos_0.y,
                localPos_0.z);

            pingPongRoutine = StartCoroutine(pingPongSequence(i_singleLerpTimeMax, i_singleLerpTimeMin, localPos_1, localPos_0, remainingLerps));
        }
        else if (resetOnFinish)
        {
            if (lerpsDone % 2 == 0)
                tr.localPosition = new Vector3(
                originalLocalPos.x,
                originalLocalPos.y,
                originalLocalPos.z);
            else if (lerpsDone > 1)
            {
                pingPongRoutine = StartCoroutine(pingPongSequence(i_singleLerpTimeMax, i_singleLerpTimeMin, localPos_1, localPos_0, 1));
            }

        }
    }
    #endregion
}
