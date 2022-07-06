using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchToJump : Action
{
    [SerializeField] StateMachine stateMachine;
    [SerializeField] GenericState jumpState;

    public override void PerformAction()
    {
        if (Input.GetKey(KeyCode.Space))
            stateMachine.SetState(jumpState);
    }
}
