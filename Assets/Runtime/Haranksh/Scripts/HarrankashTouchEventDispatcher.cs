using System;
using UnityEngine;

public class HarrankashTouchEventDispatcher : MonoBehaviourBase
{
    public Action OnTouchOrange = null;
    public Action OnTouchCart = null;

    public void DispatchOrangeTouchEvent()
    {
        OnTouchOrange?.Invoke();
    }

    public void DispatchCartTouchEvent()
    {
        OnTouchCart?.Invoke();
    }
}
