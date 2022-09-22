using System.Collections;
using UnityEngine;
public class LocalRotationPingPong : MonoBehaviourBase
{
    [SerializeField] private Transform tr = null;
    [SerializeField] private Vector3 baseRotation = Vector3.zero;
    [SerializeField] private Vector3 targetRotation = Vector3.zero;
    [SerializeField] private InterpolatorsManager interpolatorsManager = null;
    [SerializeField] private AnimationCurve singleLerpCurve = null;
    [SerializeField] private bool resetRotationOnFinish = false;
    [SerializeField] bool clampValues = true;



    private ITypedAnimator<Vector3> rotationInterpolator = null;
    private Coroutine pingPongRoutine = null;
    private Quaternion originalRotation = Quaternion.identity;

    private int lerpsDone = 0;

    private void OnDestroy()
    {
        StopPingPong();
    }

    #region PUBLIC API
    public bool isRotating => pingPongRoutine != null;

    public void StartPingPong(float i_singleLerpTime,
                              Vector3? i_baseRotation,
                              Vector3? i_targetRotation,
                              int i_numberOfLerps,
                              bool i_resetRotationOnFinish)
    {
        resetRotationOnFinish = i_resetRotationOnFinish;

        if (pingPongRoutine == null)
        {
            originalRotation = tr.rotation;
            lerpsDone = 0;
            pingPongRoutine = StartCoroutine(pingPongSequence(i_singleLerpTime, i_baseRotation, i_targetRotation, i_numberOfLerps));
        }
    }

    [ExposePublicMethod]
    public void StartPingPong(float i_singleLerpTime,
                              int i_numberOfLerps)
    {
        if (pingPongRoutine == null)
        {
            originalRotation = tr.rotation;
            lerpsDone = 0;
            pingPongRoutine = StartCoroutine(pingPongSequence(i_singleLerpTime, null, null, i_numberOfLerps));
        }
    }

    [ExposePublicMethod]
    public void StopPingPong()
    {
        this.DisposeCoroutine(ref pingPongRoutine);

        if (resetRotationOnFinish)
            tr.rotation = originalRotation;
    }

    [ExposePublicMethod]
    public void PausePingPong()
    {
        if (rotationInterpolator != null)
            rotationInterpolator.Pause();
    }

    [ExposePublicMethod]
    public void ResumePingPong()
    {
        if (rotationInterpolator != null)
            rotationInterpolator.Resume();
    }
    #endregion

    #region PRIVATE 
    private IEnumerator pingPongSequence(float i_singleLerpTime,
                                         Vector3? i_baseRotation,
                                         Vector3? i_targetRotation,
                                         int i_numberOfLerps)
    {
        int remainingLerps = Mathf.Clamp(i_numberOfLerps - 1, -1, int.MaxValue);
        AnimationMode mode = new AnimationMode(singleLerpCurve);

        Vector3 scale_0 = i_baseRotation != null ? i_baseRotation.Value : baseRotation;
        Vector3 scale_1 = i_targetRotation != null ? i_targetRotation.Value : targetRotation;

        rotationInterpolator = interpolatorsManager.Animate(scale_0, scale_1, i_singleLerpTime, mode, clampValues, 0f, null);

        while (rotationInterpolator.IsActive)
        {
            Quaternion temp = tr.rotation;
            temp.eulerAngles = rotationInterpolator.Current;
            tr.rotation = temp;

            yield return null;
        }

        this.DisposeCoroutine(ref pingPongRoutine);
        lerpsDone++;

        if (remainingLerps != 0)
            pingPongRoutine = StartCoroutine(pingPongSequence(i_singleLerpTime, scale_1, scale_0, remainingLerps));
        else if (resetRotationOnFinish)
        {
            if(lerpsDone % 2 == 0)
                tr.rotation = originalRotation;
            else
            {
                pingPongRoutine = StartCoroutine(pingPongSequence(i_singleLerpTime, scale_1, scale_0, 1));
            }
        }
    }
    #endregion
}
