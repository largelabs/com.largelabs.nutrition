using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lerper<T>
{
    T begin;
    T end;
    T current;
    float end_time;
    public Lerper(T b, T e, float et){ begin = b; end = e; end_time = et; current = b; }
    public float GetEndTime(){ return end_time; }
    public T GetCurrent(){ return current; }
    public T GetBegin() { return begin; }
    public T GetEnd() { return end; }
    public void SetCurrent(T c){ current = c; }
}
