using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpolators : MonoBehaviour
{
    public Lerper<float> Lerp_float(float begin, float end, float time_from_a_to_b)
    {
        Lerper<float> fl = new Lerper<float>(begin, end, time_from_a_to_b);
        StartCoroutine(Lerper_float(fl));
        return fl;
    }

    IEnumerator Lerper_float(Lerper<float> c)
    {
        float timer = 0f;
        float end = c.GetEndTime();
        float a = c.GetBegin();
        float b = c.GetEnd();
        while(timer < end)
        {
            c.SetCurrent(Mathf.Lerp(a, b, (timer/end)));
            timer += Time.deltaTime;
            yield return null;
        }
        c.SetCurrent(b);
    }

    public Lerper<Vector2> Lerp_Vector2(Vector2 begin, Vector2 end, float time_from_a_to_b)
    {
        Lerper<Vector2> fl = new Lerper<Vector2>(begin, end, time_from_a_to_b);
        StartCoroutine(Lerper_Vector2(fl));
        return fl;
    }

    IEnumerator Lerper_Vector2(Lerper<Vector2> c)
    {
        float timer = 0f;
        float end = c.GetEndTime();
        Vector2 a = c.GetBegin();
        Vector2 b = c.GetEnd();
        while (timer < end)
        {
            c.SetCurrent(new Vector2(Mathf.Lerp(a.x, b.x, (timer / end)), Mathf.Lerp(a.y, b.y, (timer / end))));
            timer += Time.deltaTime;
            yield return null;
        }
        c.SetCurrent(b);
    }

    public Lerper<Vector3> Lerp_Vector3(Vector3 begin, Vector3 end, float time_from_a_to_b)
    {
        Lerper<Vector3> fl = new Lerper<Vector3>(begin, end, time_from_a_to_b);
        StartCoroutine(Lerper_Vector3(fl));
        return fl;
    }

    IEnumerator Lerper_Vector3(Lerper<Vector3> c)
    {
        float timer = 0f;
        float end = c.GetEndTime();
        Vector3 a = c.GetBegin();
        Vector3 b = c.GetEnd();
        while (timer < end)
        {
            c.SetCurrent(new Vector3(Mathf.Lerp(a.x, b.x, (timer / end)), Mathf.Lerp(a.y, b.y, (timer / end)), Mathf.Lerp(a.z, b.z, (timer / end))));
            timer += Time.deltaTime;
            yield return null;
        }
        c.SetCurrent(b);
    }
}   
