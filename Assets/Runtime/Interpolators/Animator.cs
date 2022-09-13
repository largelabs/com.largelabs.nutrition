using System;
using UnityEngine;

public interface IAnimator : IPoolable
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

    bool IsClamped { get; }

    AnimationCurve AnimationFunction { get; }
    void Activate();
    void Deactivate();

    void Pause();

    void Resume();

    float AnimationTime { get; }
}

public interface ITypedAnimator<T> : IAnimator
{
    T Current { get; }

    T Start { get; }

    T Target { get; }
}



public class Animator<T> : ITypedAnimator<T>
{
    protected T start = default;
    protected T target = default;
    protected bool clamped = true;
    protected AnimationCurve animationFunction = null;
    protected Action<ITypedAnimator<T>> onAnimationEnded = null;

    T current = default;
    float animationTime = 0f;
    bool isActive = false;
    bool isAnimating = false;
    bool isPaused = false;
    float delay = 0f;

    #region CTOR

    public Animator(T i_start, T i_target, float i_animTime, AnimationCurve i_animationCurve, bool i_clamped, float i_delay = 0f, Action<ITypedAnimator<T>> i_onAnimationEnded = null)
    { 
        start = i_start; 
        target = i_target; 
        animationTime = i_animTime;
        current = i_start;
        isActive = true; 
        isAnimating = false; 
        clamped = i_clamped; 
        delay = i_delay; 
        animationFunction = i_animationCurve;
        onAnimationEnded = i_onAnimationEnded;
    }
    public Animator() { }

    public void setUpAnimator(T i_start, T i_target, float i_animTime, AnimationCurve i_animationCurve, bool i_clamped, float i_delay = 0f, Action<ITypedAnimator<T>> i_onAnimationEnded = null)
    {
        start = i_start;
        target = i_target;
        animationTime = i_animTime;
        current = i_start;
        isActive = true;
        isAnimating = false;
        clamped = i_clamped;
        delay = i_delay;
        animationFunction = i_animationCurve;
        onAnimationEnded = i_onAnimationEnded;
    }

    #endregion

    #region MUTABLE

    public void EnableAnimation() { isAnimating = true; }
    public void StopAnimating() { isAnimating = false; }

    public void TriggerExitCallback() { onAnimationEnded?.Invoke(this); }

    #endregion

    #region IAnimator

    public float AnimationTime => animationTime;
    public bool IsActive => isActive;
    public bool IsAnimating => isAnimating;
    public bool IsPaused => isPaused;
    public bool IsClamped => clamped;
    public void Activate() { isActive = true; }
    public void Deactivate() { isActive = false; isAnimating = false; }
    public void Pause() { isPaused = true; }
    public void Resume() { isPaused = false; }

    public float Delay => delay;
    public AnimationCurve AnimationFunction => animationFunction;

    #endregion

    #region IPoolable
    public void Reset()
    {
        start = default;
        target = default;
        clamped = true;
        animationFunction = null;
        onAnimationEnded = null;

        current = default;
        animationTime = 0f;
        isActive = false;
        isAnimating = false;
        isPaused = false;
        delay = 0f;
}
    public void Dispose()
    {
        Reset();
    }
    #endregion

    #region ITypedAnimator

    public T Current => current;
    public T Start => start;
    public T Target => target;

    #endregion

    #region MUTABLE

    public void SetCurrent(T i_currentValue) { current = i_currentValue; }
    public virtual void UpdateAnimator(float i_ratio) { }

    #endregion
}

public class FloatAnimator: Animator<float>
{
    public FloatAnimator(
        float i_start, 
        float i_target, 
        float i_animationTime,
        AnimationCurve i_animationFunction, 
        bool i_clamped, 
        float i_delay = 0f,
        Action<ITypedAnimator<float>> i_onAnimationEnded = null) :
        base(i_start, i_target, i_animationTime, i_animationFunction, i_clamped, i_delay, i_onAnimationEnded)
    { }

    public FloatAnimator() : base(){}

    public override void UpdateAnimator(float ratio)
    {
        SetCurrent(clamped ?    Mathf.Lerp(start, target, animationFunction.Evaluate(ratio)) :
                                Mathf.LerpUnclamped(start, target, animationFunction.Evaluate(ratio)));
    }

}
public class V2Animator : Animator<Vector2>
{
    public V2Animator(
        Vector2 i_start, 
        Vector2 i_target, 
        float i_animationTime, 
        AnimationCurve i_animationFunction, 
        bool i_clamped, 
        float i_delay = 0f,
        Action<ITypedAnimator<Vector2>> i_onAnimationEnded = null) :
        base(i_start, i_target, i_animationTime, i_animationFunction, i_clamped, i_delay, i_onAnimationEnded)
    { }

    public V2Animator() : base(){}

    public override void UpdateAnimator(float ratio)
    {
        SetCurrent(clamped ?    Vector2.Lerp(start, target, animationFunction.Evaluate(ratio)) :
                                Vector2.LerpUnclamped(start, target, animationFunction.Evaluate(ratio)));
    }

}
public class V3Animator : Animator<Vector3>
{
    public V3Animator(
        Vector3 i_start, 
        Vector3 i_target, 
        float i_animationTime, 
        AnimationCurve i_animationFunction, 
        bool i_clamped, 
        float i_delay = 0f,
        Action<ITypedAnimator<Vector3>> i_onAnimationEnded = null) :
        base(i_start, i_target, i_animationTime, i_animationFunction, i_clamped, i_delay, i_onAnimationEnded)
    { }

    public V3Animator() : base(){}
    public override void UpdateAnimator(float ratio) 
    {
        SetCurrent(clamped ?    Vector3.Lerp(start, target, animationFunction.Evaluate(ratio)) : 
                                Vector3.LerpUnclamped(start, target, animationFunction.Evaluate(ratio))); 
    }

}
public class ColorAnimator : Animator<Color>
{
    public ColorAnimator(
        Color i_start, 
        Color i_target, 
        float i_animationTime, 
        AnimationCurve i_animationFunction, 
        bool i_clamped, 
        float i_delay = 0f,
        Action<ITypedAnimator<Color>> i_onAnimationEnded = null) : 
        base(i_start, i_target, i_animationTime, i_animationFunction, i_clamped, i_delay, i_onAnimationEnded) 
    { }

    public ColorAnimator() : base(){}

    public override void UpdateAnimator(float ratio)
    {
        SetCurrent(clamped ?    Color.Lerp(start, target, animationFunction.Evaluate(ratio)) :
                                Color.LerpUnclamped(start, target, animationFunction.Evaluate(ratio)));
    }
}
