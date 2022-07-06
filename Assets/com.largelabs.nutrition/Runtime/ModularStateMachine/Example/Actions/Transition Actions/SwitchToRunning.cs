using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchToRunning : Action
{
    public StateMachine stateMachine;
    public GenericState RUNNING;
    public SetVelocity setVelocity;
    public override void PerformAction()
    {
        if (stateMachine is null)
            return;
        if (RUNNING is null)
            return;
        if (setVelocity is null)
            return;

        if (Input.GetKey(KeyCode.LeftShift) && setVelocity.direction != 0)
            stateMachine.SetState(RUNNING);
    }
}
