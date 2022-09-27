using System.Collections;
using UnityEngine;

public class LocalPositionPingPong : MonoBehaviourBase
{
    [SerializeField] private Transform tr = null;
    [SerializeField] private Vector3 baseLocalPos = Vector3.zero;
    [SerializeField] private Vector3 targetLocalPos = Vector3.zero;
    [SerializeField] private InterpolatorsManager interpolatorsManager = null;
    [SerializeField] private AnimationCurve singleLerpCurve = null;
    [SerializeField] private bool clampValues = true;
    [SerializeField] private bool resetOnFinish = true;

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

    public void StartPingPong(float i_singleLerpTime,
                              Vector3? i_baseLocalPos,
                              Vector3? i_targetLocalPos,
                              int i_numberOfLerps,
                              bool i_resetColorOnFinish)
    {
        resetOnFinish = i_resetColorOnFinish;
        if (pingPongRoutine == null)
        {
            originalLocalPos = tr.localPosition;
            lerpsDone = 0;
            pingPongRoutine = StartCoroutine(pingPongSequence(i_singleLerpTime, i_baseLocalPos, i_targetLocalPos, i_numberOfLerps));
        }
    }

    [ExposePublicMethod]
    public void StartPingPong(float i_singleLerpTime,
                              int i_numberOfLerps)
    {
        if (pingPongRoutine == null)
        {
            originalLocalPos = tr.localPosition;
            lerpsDone = 0;
            pingPongRoutine = StartCoroutine(pingPongSequence(i_singleLerpTime, null, null, i_numberOfLerps));
        }
    }

    [ExposePublicMethod]
    public void StopPingPong()
    {
        this.DisposeCoroutine(ref pingPongRoutine);

        if (resetOnFinish)
            tr.localPosition = originalLocalPos;
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
    private IEnumerator pingPongSequence(float i_singleLerpTime,
                                         Vector3? i_baseLocalPos,
                                         Vector3? i_targetLocalPos,
                                         int i_numberOfLerps)
    {
        int remainingLerps = Mathf.Clamp(i_numberOfLerps - 1, -1, int.MaxValue);
        AnimationMode mode = new AnimationMode(singleLerpCurve);

        Vector3 localPos_0 = i_baseLocalPos != null ? i_baseLocalPos.Value : baseLocalPos;
        Vector3 localPos_1 = i_targetLocalPos != null ? i_targetLocalPos.Value : targetLocalPos;

        localPosInterpolator = interpolatorsManager.Animate(localPos_0, localPos_1, i_singleLerpTime, mode, clampValues, 0f, null);

        while (localPosInterpolator.IsActive)
        {
            tr.localPosition = localPosInterpolator.Current;
            yield return null;
        }

        this.DisposeCoroutine(ref pingPongRoutine);
        lerpsDone++;

        if (remainingLerps != 0)
            pingPongRoutine = StartCoroutine(pingPongSequence(i_singleLerpTime, localPos_1, localPos_0, remainingLerps));
        else if (resetOnFinish)
        {
            if (lerpsDone % 2 == 0)
                tr.localPosition = originalLocalPos;
            else
            {
                pingPongRoutine = StartCoroutine(pingPongSequence(i_singleLerpTime, localPos_1, localPos_0, 1));
            }
            
        }
    }
    #endregion
}
