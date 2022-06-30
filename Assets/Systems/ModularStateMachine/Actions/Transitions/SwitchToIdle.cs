using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchToIdle : Action
{
    public StateMachine stateMachine;
    public State IDLE;
    public SetVelocity setVelocity;
    public override void performAction()
    {
        if (stateMachine is null)
            return;
        if (IDLE is null)
            return;
        if (setVelocity is null)
            return;

        if (setVelocity.direction == 0)
            stateMachine.SetState(IDLE);
    }
}
