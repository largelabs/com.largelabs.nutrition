using System.Collections;
using UnityEngine;

public static class MonoBehaviourExtensions
{
    public static void DisposeCoroutine(this MonoBehaviour i_behaviour, ref Coroutine i_routine)
    {
        if (null != i_routine)
        {
            i_behaviour.StopCoroutine(i_routine);
            i_routine = null;
        }
    }

    public static IEnumerator Wait(this MonoBehaviour i_behaviour, float i_waitTime, bool i_realTime = false)
    {
        if (i_waitTime > 0)
        {
            if (false == i_realTime)
                yield return new WaitForSeconds(i_waitTime);
            else
                yield return new WaitForSecondsRealtime(i_waitTime);
        }
    }
}