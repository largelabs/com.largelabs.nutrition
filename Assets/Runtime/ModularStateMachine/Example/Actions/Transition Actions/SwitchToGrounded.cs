using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchToGrounded : Action
{
    [SerializeField] StateMachine stateMachine;
    [SerializeField] GenericState groundedState;
    [SerializeField] Rigidbody rb;
    public override void PerformAction()
    {
        if (rb.velocity.y == 0)
            stateMachine.SetState(groundedState);
    }
}
