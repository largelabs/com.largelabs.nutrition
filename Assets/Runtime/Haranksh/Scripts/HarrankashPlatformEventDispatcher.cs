using System;
using UnityEngine;

public class HarrankashPlatformEventDispatcher : MonoBehaviourBase
{
    public Action<Vector3> OnTouchOrange = null;
    public Action<Vector3> OnTouchNormal = null;
    public Action OnFailConditionMet = null;

    public void DispatchOrangeTouchEvent(Vector3 i_platformPos)
    {
        OnTouchOrange?.Invoke(i_platformPos);
    }  
    
    public void DispatchNormalTouchEvent(Vector3 i_platformPos)
    {
        OnTouchNormal?.Invoke(i_platformPos);
    }

    public void DispatchFailGameEvent()
    {
        OnFailConditionMet?.Invoke();
    }
}
