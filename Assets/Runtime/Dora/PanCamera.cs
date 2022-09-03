using System;
using System.Collections;
using UnityEngine;

public class PanCamera : MonoBehaviourBase
{
    [SerializeField] private Transform posDown = null;
    [SerializeField] private Transform posUp = null;
    [SerializeField] private InterpolatorsManager interpolatorManager = null;
    [SerializeField] private AnimationCurve panCurve = null;

    Coroutine panCameraRoutine = null;

    [ExposePublicMethod]
    public void PanCameraWithPos(Vector3 i_cameraPos, float i_panTime)
    {
        if(panCameraRoutine == null)
            panCameraRoutine = StartCoroutine(animateToTransform(transform, i_cameraPos, i_panTime,
                                            panCurve, null));
    } 
    
    [ExposePublicMethod]
    public void PanCameraDown(float i_panTime)
    {
        if(panCameraRoutine == null)
            panCameraRoutine = StartCoroutine(animateToTransform(transform, posDown.position, i_panTime,
                                            panCurve, null));
    }
    
    [ExposePublicMethod]
    public void PanCameraUp(float i_panTime)
    {
        if(panCameraRoutine == null)
            panCameraRoutine = StartCoroutine(animateToTransform(transform, posUp.position, i_panTime,
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
