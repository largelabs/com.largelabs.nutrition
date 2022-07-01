using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleStateName : Action
{
    public override void PerformAction()
    {
        Debug.Log(gameObject.name);
    }
}
