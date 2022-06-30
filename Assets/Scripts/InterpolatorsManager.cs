using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterpolatorsManager : MonoBehaviour
{
    Dictionary<Animator<float>,Coroutine> animators_float = new Dictionary<Animator<float>,Coroutine>();
    Dictionary<Animator<Vector2>, Coroutine> animators_V2 = new Dictionary<Animator<Vector2>, Coroutine>();
    Dictionary<Animator<Vector3>, Coroutine> animators_V3 = new Dictionary<Animator<Vector3>, Coroutine>();
    Dictionary<Animator<Color>, Coroutine> animators_Color = new Dictionary<Animator<Color>, Coroutine>();
    public Animator<float> Animate(float begin, float end, float time_from_a_to_b,AnimationMode Mode, float Delay_at_the_start = 0)
    {
        Animator<float> fl = new Animator<float>(begin, end, time_from_a_to_b, Mode.Curve,Delay_at_the_start);
        Coroutine coroutine = StartCoroutine(Interpolat(fl));
        animators_float.Add(fl,coroutine);
        return fl;
    }
    public Animator<Vector2> Animate(Vector2 begin, Vector2 end, float time_from_a_to_b, AnimationMode Mode, float Delay_at_the_start = 0)
    {
        Animator<Vector2> fl = new Animator<Vector2>(begin, end, time_from_a_to_b,Mode.Curve, Delay_at_the_start);
        Coroutine coroutine = StartCoroutine(Interpolat(fl));
        animators_V2.Add(fl, coroutine);
        return fl;
    }
    public Animator<Vector3> Animate(Vector3 begin, Vector3 end, float time_from_a_to_b, AnimationMode Mode, float Delay_at_the_start = 0)
    {
        Animator<Vector3> fl = new Animator<Vector3>(begin, end, time_from_a_to_b,Mode.Curve, Delay_at_the_start);
        Coroutine coroutine = StartCoroutine(Interpolat(fl));
        animators_V3.Add(fl, coroutine);
        return fl;
    }
    public Animator<Color> Animate(Color begin, Color end, float time_from_a_to_b, AnimationMode Mode, float Delay_at_the_start = 0)
    {
        Animator<Color> fl = new Animator<Color>(begin, end, time_from_a_to_b,Mode.Curve, Delay_at_the_start);
        Coroutine coroutine = StartCoroutine(Interpolat(fl));
        animators_Color.Add(fl, coroutine);
        return fl;
    }
    public void Stop(Animator<float> fl)
    {
        if (animators_float.ContainsKey(fl))
        {
            StopCoroutine(animators_float[fl]);
            animators_float.Remove(fl);
        }
    }
    public void Stop(Animator<Vector2> fl)
    {
        if (animators_V2.ContainsKey(fl))
        {
            StopCoroutine(animators_V2[fl]);
            animators_V2.Remove(fl);
        }
    }
    public void Stop(Animator<Vector3> fl)
    {
        if (animators_V3.ContainsKey(fl))
        {
            StopCoroutine(animators_V3[fl]);
            animators_V3.Remove(fl);
        }
    }
    public void Stop(Animator<Color> fl)
    {
        if (animators_Color.ContainsKey(fl))
        {
            StopCoroutine(animators_Color[fl]);
            animators_Color.Remove(fl);
        }
    }
    public void StopAllAnimations()
    {
        StopAllCoroutines();
        animators_float.Clear();
        animators_V2.Clear();
        animators_V3.Clear();
        animators_Color.Clear();
    }

    IEnumerator Interpolat(Animator<float> c)
    {
        yield return new WaitForSeconds(c.Delay);
        c.Resume();
        float timer = 0f;
        float end = c.EndTime;
        float a = c.Begin;
        float b = c.End;
        AnimationCurve function = c.Function;
        while (timer < end)
        {
            while (!c.IsAnimating)
            {
                yield return null;
            }
            c.SetCurrent(Mathf.Lerp(a, b, function.Evaluate(timer / end)));
            timer += Time.deltaTime;
            yield return null;
        }
        c.SetCurrent(Mathf.Lerp(a, b, function.Evaluate(1)));
        c.Pause();
        animators_float.Remove(c);
    }

    IEnumerator Interpolat(Animator<Vector2> c)
    {
        yield return new WaitForSeconds(c.Delay);
        c.Resume();
        float timer = 0f;
        float end = c.EndTime;
        Vector2 a = c.Begin;
        Vector2 b = c.End;
        AnimationCurve function = c.Function;
        while (timer < end)
        {
            while (!c.IsAnimating)
            {
                yield return null;
            }
            c.SetCurrent(Vector2.Lerp(a, b, function.Evaluate(timer / end)));
            timer += Time.deltaTime;
            yield return null;
        }
        c.SetCurrent(Vector2.Lerp(a, b, function.Evaluate(1)));
        c.Pause();
        animators_V2.Remove(c);
    }

    IEnumerator Interpolat(Animator<Vector3> c)
    {
        yield return new WaitForSeconds(c.Delay);
        c.Resume();
        float timer = 0f;
        float end = c.EndTime;
        Vector3 a = c.Begin;
        Vector3 b = c.End;
        AnimationCurve function = c.Function;
        while (timer < end)
        {
            while (!c.IsAnimating)
            {
                yield return null;
            }
            c.SetCurrent(Vector3.Lerp(a, b, function.Evaluate(timer / end)));
            timer += Time.deltaTime;
            yield return null;
        }
        c.SetCurrent(Vector3.Lerp(a, b, function.Evaluate(1)));
        c.Pause();
        animators_V3.Remove(c);
    }

    IEnumerator Interpolat(Animator<Color> c)
    {
        yield return new WaitForSeconds(c.Delay);
        c.Resume();
        float timer = 0f;
        float end = c.EndTime;
        Color a = c.Begin;
        Color b = c.End;
        AnimationCurve function = c.Function;
        while (timer < end)
        {
            while (!c.IsAnimating)
            {
                yield return null;
            }
            c.SetCurrent(Color.Lerp(a, b, function.Evaluate(timer / end)));
            timer += Time.deltaTime;
            yield return null;
        }
        c.SetCurrent(Color.Lerp(a, b, function.Evaluate(1)));
        c.Pause();
        animators_Color.Remove(c);
    }
}