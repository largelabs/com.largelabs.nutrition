using System;
using UnityEngine;

public class HarrankashTouchEventDispatcher : MonoBehaviourBase
{
    public Action<Vector3> OnTouchOrange = null;
    public Action OnTouchCart = null;

    public void DispatchOrangeTouchEvent(Vector3 i_platformPos)
    {
        OnTouchOrange?.Invoke(i_platformPos);
    }

    public void DispatchCartTouchEvent()
    {
        OnTouchCart?.Invoke();
    }
}
