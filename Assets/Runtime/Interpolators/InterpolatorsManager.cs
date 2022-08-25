using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InterpolatorsManager : MonoBehaviourBase
{
    [SerializeField] int maxConcurrentFloat = -1;
    [SerializeField] int maxConcurrentV2 = -1;
    [SerializeField] int maxConcurrentV3 = -1;
    [SerializeField] int maxConcurrentColor = -1;

    Dictionary<IAnimator, Coroutine> animators = new Dictionary<IAnimator, Coroutine>();

    private ManagedPool<FloatAnimator> floatAnimationPool;
    private ManagedPool<V2Animator> v2AnimationPool;
    private ManagedPool<V3Animator> v3AnimationPool;
    private ManagedPool<ColorAnimator> colorAnimationPool;

    public ITypedAnimator<float> Animate(float i_start, float i_target, float i_time, AnimationMode i_interpolationMode, bool i_clamped = true, float i_delay = 0, Action<ITypedAnimator<float>> i_onAnimationEnded = null)
    {
        if(null == floatAnimationPool) floatAnimationPool = new ManagedPool<FloatAnimator>(maxConcurrentFloat);

        FloatAnimator fl = floatAnimationPool.GetItem();
        fl.setUpAnimator(i_start, i_target, i_time, i_interpolationMode.Curve, i_clamped, i_delay, i_onAnimationEnded);
        Coroutine coroutine = StartCoroutine(Interpolat(fl));
        animators.Add(fl, coroutine);
        return fl;
    }
    public ITypedAnimator<Vector2> Animate(Vector2 i_start, Vector2 i_target, float i_time, AnimationMode i_interpolationMode, bool i_clamped = true, float i_delay = 0, Action<ITypedAnimator<Vector2>> i_onAnimationEnded = null)
    {
        if (null == v2AnimationPool) v2AnimationPool = new ManagedPool<V2Animator>(maxConcurrentV2);

        V2Animator fl = v2AnimationPool.GetItem();
        fl.setUpAnimator(i_start, i_target, i_time, i_interpolationMode.Curve, i_clamped, i_delay, i_onAnimationEnded);
        Coroutine coroutine = StartCoroutine(Interpolat(fl));
        animators.Add(fl, coroutine);
        return fl;
    }
    public ITypedAnimator<Vector3> Animate(Vector3 i_start, Vector3 i_target, float i_time, AnimationMode i_interpolationMode, bool i_clamped = true, float i_delay = 0, Action<ITypedAnimator<Vector3>> i_onAnimationEnded = null)
    {
        if (null == v3AnimationPool) v3AnimationPool = new ManagedPool<V3Animator>(maxConcurrentV3);

        V3Animator fl = v3AnimationPool.GetItem();
        fl.setUpAnimator(i_start, i_target, i_time, i_interpolationMode.Curve, i_clamped, i_delay, i_onAnimationEnded);
        Coroutine coroutine = StartCoroutine(Interpolat(fl));
        animators.Add(fl, coroutine);
        return fl;
    }
    public ITypedAnimator<Color> Animate(Color i_start, Color i_target, float i_time, AnimationMode i_interpolationMode, bool i_clamped = true, float i_delay = 0, Action<ITypedAnimator<Color>> i_onAnimationEnded = null)
    {
        if (null == colorAnimationPool) colorAnimationPool = new ManagedPool<ColorAnimator>(maxConcurrentColor);

        ColorAnimator fl = colorAnimationPool.GetItem();
        fl.setUpAnimator(i_start, i_target, i_time, i_interpolationMode.Curve, i_clamped, i_delay, i_onAnimationEnded);
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
            return_to_pool(fl);
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
        floatAnimationPool.ResetAll();
        v2AnimationPool.ResetAll();
        v3AnimationPool.ResetAll();
        colorAnimationPool.ResetAll();
    }

    IEnumerator Interpolat<T>(Animator<T> i_animator)
    {
        i_animator.Activate();
        float timer = 0f;
        float delay = i_animator.Delay;

        while (timer < delay)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0f;
        float animationTime = i_animator.AnimationTime;
        while (timer < animationTime)
        {
            while (i_animator.IsPaused)
            {
                i_animator.StopAnimating();
                yield return null;
            }
            i_animator.EnableAnimation();
            i_animator.UpdateAnimator(timer / animationTime);
            timer += Time.deltaTime;
            yield return null;
        }
        i_animator.UpdateAnimator(1);
        i_animator.TriggerExitCallback();

        yield return null;
        animators.Remove(i_animator);
        return_to_pool(i_animator);
        i_animator.Deactivate();
    }

    private void return_to_pool(IAnimator fl)
    {
        if(fl.GetType() == typeof(FloatAnimator))
        {
            floatAnimationPool.ResetItem((FloatAnimator)fl);
        }
        else if (fl.GetType() == typeof(V2Animator))
        {
            v2AnimationPool.ResetItem((V2Animator)fl);
        }
        else if (fl.GetType() == typeof(V3Animator))
        {
            v3AnimationPool.ResetItem((V3Animator)fl);
        }
        else if (fl.GetType() == typeof(ColorAnimator))
        {
            colorAnimationPool.ResetItem((ColorAnimator)fl);
        }
        else
        {
            Debug.LogError("CANNOT CONVERT ANIMATOR TO CORRECT TYPE");
        }
    }
}