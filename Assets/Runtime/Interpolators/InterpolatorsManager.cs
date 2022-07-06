using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InterpolatorsManager : MonoBehaviourBase
{
    Dictionary<IAnimator, Coroutine> animators = new Dictionary<IAnimator, Coroutine>();

    public ITypedAnimator<float> Animate(float i_start, float i_target, float i_time, AnimationMode i_interpolationMode, float i_delay = 0)
    {
        FloatAnimator fl = new FloatAnimator(i_start, i_target, i_time, i_interpolationMode.Curve, i_delay);
        Coroutine coroutine = StartCoroutine(Interpolat(fl));
        animators.Add(fl, coroutine);
        return (ITypedAnimator<float>)fl;
    }
    public ITypedAnimator<Vector2> Animate(Vector2 i_start, Vector2 i_target, float i_time, AnimationMode i_interpolationMode, float i_delay = 0)
    {
        V2Animator fl = new V2Animator(i_start, i_target, i_time, i_interpolationMode.Curve, i_delay);
        Coroutine coroutine = StartCoroutine(Interpolat(fl));
        animators.Add(fl, coroutine);
        return (ITypedAnimator<Vector2>)fl;
    }
    public ITypedAnimator<Vector3> Animate(Vector3 i_start, Vector3 i_target, float i_time, AnimationMode i_interpolationMode, float i_delay = 0)
    {
        V3Animator fl = new V3Animator(i_start, i_target, i_time, i_interpolationMode.Curve, i_delay);
        Coroutine coroutine = StartCoroutine(Interpolat(fl));
        animators.Add(fl, coroutine);
        return (ITypedAnimator<Vector3>)fl;
    }
    public ITypedAnimator<Color> Animate(Color i_start, Color i_target, float i_time, AnimationMode i_interpolationMode, float i_delay = 0)
    {
        ColorAnimator fl = new ColorAnimator(i_start, i_target, i_time, i_interpolationMode.Curve, i_delay);
        Coroutine coroutine = StartCoroutine(Interpolat(fl));
        animators.Add(fl, coroutine);
        return (ITypedAnimator<Color>)fl;
    }
    public void Stop(IAnimator fl)
    {
        if (animators.ContainsKey(fl))
        {
            StopCoroutine(animators[fl]);
            fl.Deactivate();
            fl.Pause();
            animators.Remove(fl);
        }
    }

    public void StopAllAnimations()
    {
        StopAllCoroutines();
        foreach (var fl in animators.Keys)
        {
            fl.Deactivate();
            fl.Pause();
        }
        animators.Clear();
    }

    IEnumerator Interpolat<T>(Animator<T> c)
    {
        c.Activate();
        yield return new WaitForSeconds(c.Delay);
        c.Resume();
        float timer = 0f;
        float end = c.EndTime;
        while (timer < end)
        {
            while (!c.IsAnimating)
            {
                yield return null;
            }
            c.UpdateAnimator(timer/end);
            timer += Time.deltaTime;
            yield return null;
        }
        c.UpdateAnimator(1);
        yield return null;
        c.Pause();
        animators.Remove(c);
        c.Deactivate();
    }

}