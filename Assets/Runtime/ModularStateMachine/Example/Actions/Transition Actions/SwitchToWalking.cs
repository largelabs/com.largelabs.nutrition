using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchToWalking : Action
{
    public StateMachine stateMachine;
    public GenericState WALKING;
    public SetVelocity setVelocity;
    public override void PerformAction()
    {
        if (stateMachine is null)
            return;
        if (WALKING is null)
            return;
        if (setVelocity is null)
            return;

        if (setVelocity.direction != 0 && !Input.GetKey(KeyCode.LeftShift))
            stateMachine.SetState(WALKING);
    }
}
