using System;
using System.Collections;
using UnityEngine;

public class PanCamera : MonoBehaviourBase
{
    [SerializeField] private Camera cam = null;
    [SerializeField] private Transform posDown = null;
    [SerializeField] private Transform posUp = null;
    [SerializeField] private InterpolatorsManager interpolatorManager = null;
    [SerializeField] private AnimationCurve panCurve = null;
    [SerializeField] private float maxOrthoSize = 5.4f;
    [SerializeField] private float minOrthoSize = 4.12f;

    [SerializeField] private float panDownTime = 0.2f;
    [SerializeField] private float panUpTime = 0.5f;

    Coroutine panCameraRoutine = null;

    public bool IsMovingCamera => null != panCameraRoutine;

    public float PanUpTime => panUpTime;

    public float PanDownTime => panDownTime;


    public void SetOrthoSize(float i_t)
    {
        Debug.Log(i_t);
        cam.orthographicSize = Mathf.Lerp(maxOrthoSize, minOrthoSize, i_t);
    }

    [ExposePublicMethod]
    public void PanCameraDown()
    {
        if(panCameraRoutine == null)
            panCameraRoutine = StartCoroutine(animateToTransform(transform, posDown.position, panDownTime,
                                            panCurve, null));
    }
    
    [ExposePublicMethod]
    public void PanCameraUp()
    {
        if(panCameraRoutine == null)
            panCameraRoutine = StartCoroutine(animateToTransform(transform, posUp.position, panUpTime,
                                            panCurve, null));
    }

    IEnumerator animateToTransform(Transform i_camera, Vector3 i_target, float i_time, AnimationCurve i_curve, Action<ITypedAnimator<Vector3>> i_onAnimationEnded)
    {
        AnimationMode mode = new AnimationMode(i_curve);
        ITypedAnimator<Vector3> posInterpolator = interpolatorManager.Animate(i_camera.position, i_target, i_time, mode, false, 0f, i_onAnimationEnded);

        while (true == posInterpolator.IsActive)
        {
            i_camera.position = posInterpolator.Current;
            yield return null;
        }

        this.DisposeCoroutine(ref panCameraRoutine);
    }
}
