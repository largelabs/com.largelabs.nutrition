using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchToGrounded : Action
{
    [SerializeField] StateMachine stateMachine;
    [SerializeField] GenericState groundedState;
    [SerializeField] Rigidbody rigidbody;
    public override void PerformAction()
    {
        if (rigidbody.velocity.y == 0)
            stateMachine.SetState(groundedState);
    }
}
