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
        return fl;
    }
    public ITypedAnimator<Vector2> Animate(Vector2 i_start, Vector2 i_target, float i_time, AnimationMode i_interpolationMode, float i_delay = 0)
    {
        V2Animator fl = new V2Animator(i_start, i_target, i_time, i_interpolationMode.Curve, i_delay);
        Coroutine coroutine = StartCoroutine(Interpolat(fl));
        animators.Add(fl, coroutine);
        return fl;
    }
    public ITypedAnimator<Vector3> Animate(Vector3 i_start, Vector3 i_target, float i_time, AnimationMode i_interpolationMode, float i_delay = 0)
    {
        V3Animator fl = new V3Animator(i_start, i_target, i_time, i_interpolationMode.Curve, i_delay);
        Coroutine coroutine = StartCoroutine(Interpolat(fl));
        animators.Add(fl, coroutine);
        return fl;
    }
    public ITypedAnimator<Color> Animate(Color i_start, Color i_target, float i_time, AnimationMode i_interpolationMode, float i_delay = 0)
    {
        ColorAnimator fl = new ColorAnimator(i_start, i_target, i_time, i_interpolationMode.Curve, i_delay);
        Coroutine coroutine = StartCoroutine(Interpolat(fl));
        animators.Add(fl, coroutine);
        return fl;
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
        float timer = 0f;
        float delay = c.Delay;

        while(timer < delay)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0f;        
        float end = c.EndTime;
        while (timer < end)
        {
            while (c.IsPaused)
            {
                c.stopAnimating();
                yield return null;
            }
            c.Animating();
            c.UpdateAnimator(timer/end);
            timer += Time.deltaTime;
            yield return null;
        }
        c.UpdateAnimator(1);
        yield return null;
        animators.Remove(c);
        c.Deactivate();
    }

}