using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnimator
{
    // Add IsPaused

    bool IsAnimating { get; }
    float Delay { get; }

    AnimationCurve Function { get; }

    void Pause();

    void Resume();

    float EndTime { get; }
}

public interface ITypedAnimator<T> : IAnimator
{
    T Current { get; }

    T Begin { get; }

    T Target { get; }
}



public class Animator<T> : ITypedAnimator<T>
{
    T begin;
    T end;
    T current;
    float end_time;
    bool is_animating;
    float delay;
    AnimationCurve function;

    #region CTOR

    public Animator(T b, T e, float et, AnimationCurve a, float d=0f){ begin = b; end = e; end_time = et; current = b; is_animating = false; delay = d; function = a; }

    #endregion

    #region IAnimator

    public float EndTime => end_time;
    public bool IsAnimating => is_animating;
    public void Pause() { is_animating = false; }
    public void Resume() { is_animating = true; }
    public float Delay => delay;
    public AnimationCurve Function => function;

    #endregion

    #region ITypedAnimator

    public T Current => current;
    public T Begin => begin;
    public T Target => end;

    #endregion

    #region MUTABLE

    public void SetCurrent(T c){ current = c; }

    #endregion

}
