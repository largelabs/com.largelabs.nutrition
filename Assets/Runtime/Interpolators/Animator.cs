using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnimator
{
    /// <summary>
    /// True when the interpolator routine is running,
    /// whether it is paused or not
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// True when the value is being updated in the interpolator
    /// AND the interpolator is not paused
    /// </summary>
    bool IsAnimating { get; }

    /// <summary>
    /// True when the interpolator is paused by user
    /// </summary>
    bool IsPaused { get; }

    float Delay { get; }

    AnimationCurve Function { get; }
    void Activate();
    void Deactivate();

    void Pause();

    void Resume();

    float EndTime { get; }
}

public interface ITypedAnimator<T> : IAnimator
{
    T Current { get; }

    T Start { get; }

    T Target { get; }
}



public class Animator<T> : ITypedAnimator<T>
{
    protected T start;
    protected T target;
    T current;
    float endTime;
    bool isActive;
    bool isAnimating;
    bool isPaused;
    float delay;
    protected AnimationCurve function;

    #region CTOR

    public Animator(T b, T e, float et, AnimationCurve a, float d=0f){ start = b; target = e; endTime = et; current = b;isActive = true; isAnimating = false; delay = d; function = a; }

    #endregion

    #region IAnimator

    public float EndTime => endTime;
    public bool IsActive => isActive;
    public bool IsAnimating => isAnimating;
    public bool IsPaused => isPaused;
    public void Activate() { isActive = true; }
    public void Deactivate() { isActive = false; isAnimating = false; }
    public void Pause() { isPaused = true; }
    public void Resume() { isPaused = false; }
    public void Animating() { isAnimating = true; }
    public void stopAnimating() { isAnimating = false; }
    public float Delay => delay;
    public AnimationCurve Function => function;

    #endregion

    #region ITypedAnimator

    public T Current => current;
    public T Start => start;
    public T Target => target;

    #endregion

    #region MUTABLE

    public void SetCurrent(T c){ current = c; }
    public virtual void UpdateAnimator(float ratio) { }

    #endregion
}

public class FloatAnimator: Animator<float>
{
    public FloatAnimator(float b, float e, float et, AnimationCurve a, float d = 0f):base(b,e,et,a,d) { }
    public override void UpdateAnimator(float ratio) { SetCurrent(Mathf.Lerp(start, target, function.Evaluate(ratio))); }

}
public class V2Animator : Animator<Vector2>
{
    public V2Animator(Vector2 b, Vector2 e, float et, AnimationCurve a, float d = 0f) : base(b, e, et, a, d) { }
    public override void UpdateAnimator(float ratio) { SetCurrent(Vector2.Lerp(start, target, function.Evaluate(ratio))); }

}
public class V3Animator : Animator<Vector3>
{
    public V3Animator(Vector3 b, Vector3 e, float et, AnimationCurve a, float d = 0f) : base(b, e, et, a, d) { }
    public override void UpdateAnimator(float ratio) { SetCurrent(Vector3.Lerp(start, target, function.Evaluate(ratio))); }

}
public class ColorAnimator : Animator<Color>
{
    public ColorAnimator(Color b, Color e, float et, AnimationCurve a, float d = 0f) : base(b, e, et, a, d) { }
    public override void UpdateAnimator(float ratio) { SetCurrent(Color.Lerp(start, target, function.Evaluate(ratio))); }

}
