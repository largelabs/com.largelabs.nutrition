using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterpolatorsManager : MonoBehaviour
{
    Dictionary<IAnimator, Coroutine> animators = new Dictionary<IAnimator, Coroutine>();

    public ITypedAnimator<float> Animate(float i_start, float i_target, float i_time ,AnimationMode i_interpolationMode, float i_delay = 0)
    {
        Animator<float> fl = new Animator<float>(i_start, i_target, i_time, i_interpolationMode.Curve,i_delay);
        Coroutine coroutine = StartCoroutine(Interpolat(fl));
        animators.Add(fl,coroutine);
        return fl;
    }
    public Animator<Vector2> Animate(Vector2 begin, Vector2 end, float time_from_a_to_b, AnimationMode Mode, float Delay_at_the_start = 0)
    {
        Animator<Vector2> fl = new Animator<Vector2>(begin, end, time_from_a_to_b,Mode.Curve, Delay_at_the_start);
        Coroutine coroutine = StartCoroutine(Interpolat(fl));
        animators.Add(fl, coroutine);
        return fl;
    }
    public Animator<Vector3> Animate(Vector3 begin, Vector3 end, float time_from_a_to_b, AnimationMode Mode, float Delay_at_the_start = 0)
    {
        Animator<Vector3> fl = new Animator<Vector3>(begin, end, time_from_a_to_b,Mode.Curve, Delay_at_the_start);
        Coroutine coroutine = StartCoroutine(Interpolat(fl));
        animators.Add(fl, coroutine);
        return fl;
    }
    public Animator<Color> Animate(Color begin, Color end, float time_from_a_to_b, AnimationMode Mode, float Delay_at_the_start = 0)
    {
        Animator<Color> fl = new Animator<Color>(begin, end, time_from_a_to_b,Mode.Curve, Delay_at_the_start);
        Coroutine coroutine = StartCoroutine(Interpolat(fl));
        animators.Add(fl, coroutine);
        return fl;
    }
    public void Stop(IAnimator fl)
    {
        if (animators.ContainsKey(fl))
        {
            StopCoroutine(animators[fl]);
            animators.Remove(fl);
        }
    }

    public void StopAllAnimations()
    {
        StopAllCoroutines();
        animators.Clear();
    }

    IEnumerator Interpolat(Animator<float> c)
    {
        yield return new WaitForSeconds(c.Delay);
        c.Resume();
        float timer = 0f;
        float end = c.EndTime;
        float a = c.Begin;
        float b = c.Target;
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
        animators.Remove(c);
    }

    IEnumerator Interpolat(Animator<Vector2> c)
    {
        yield return new WaitForSeconds(c.Delay);
        c.Resume();
        float timer = 0f;
        float end = c.EndTime;
        Vector2 a = c.Begin;
        Vector2 b = c.Target;
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
        animators.Remove(c);
    }

    IEnumerator Interpolat<T>(Animator<Vector3> c)
    {
        yield return new WaitForSeconds(c.Delay);
        c.Resume();
        float timer = 0f;
        float end = c.EndTime;
        Vector3 a = c.Begin;
        Vector3 b = c.Target;
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
        animators.Remove(c);
    }

    IEnumerator Interpolat(Animator<Color> c)
    {
        yield return new WaitForSeconds(c.Delay);
        c.Resume();
        float timer = 0f;
        float end = c.EndTime;
        Color a = c.Begin;
        Color b = c.Target;
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
        animators.Remove(c);
    }
}