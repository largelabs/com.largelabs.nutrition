using System;
using UnityEngine;

public class HarrankashPlatformEventDispatcher : MonoBehaviourBase
{
    public Action<Vector3> OnTouchOrange = null;
    public Action OnFailConditionMet = null;

    public void DispatchOrangeTouchEvent(Vector3 i_platformPos)
    {
        OnTouchOrange?.Invoke(i_platformPos);
    }

    public void DispatchFailGameEvent()
    {
        OnFailConditionMet?.Invoke();
    }
}
