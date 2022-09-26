using System;
using UnityEngine;

public class HarrankashPlatformEventDispatcher : MonoBehaviourBase
{
    public Action<Vector3> OnTouchOrange = null;
    public Action<Vector3> OnFirstTouchNormal = null;
    public Action OnFailConditionMet = null;

    public void DispatchOrangeTouchEvent(Vector3 i_platformPos)
    {
        OnTouchOrange?.Invoke(i_platformPos);
    }  
    
    public void DispatchNormalFirstTouchEvent(Vector3 i_platformPos)
    {
        OnFirstTouchNormal?.Invoke(i_platformPos);
    }

    public void DispatchFailGameEvent()
    {
        OnFailConditionMet?.Invoke();
    }
}
