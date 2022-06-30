using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleStateName : Action
{
    public override void performAction()
    {
        Debug.Log(gameObject.name);
    }
}
