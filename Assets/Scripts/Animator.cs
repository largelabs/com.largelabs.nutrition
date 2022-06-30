using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animator<T>
{
    T begin;
    T end;
    T current;
    float end_time;
    bool is_animating;
    float delay;
    AnimationCurve function;
    public Animator(T b, T e, float et, AnimationCurve a, float d=0f){ begin = b; end = e; end_time = et; current = b; is_animating = false; delay = d; function = a; }
    public float EndTime => end_time;
    public T Current => current;
    public T Begin => begin;
    public T End => end;
    public void SetCurrent(T c){ current = c; }
    public bool IsAnimating => is_animating;
    public void Pause() { is_animating = false; }
    public void Resume() { is_animating = true; }
    public float Delay => delay;
    public AnimationCurve Function => function;
}
