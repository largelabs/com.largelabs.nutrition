using System;
using System.Collections;
using UnityEngine;


public class TestWalking : State
{
    
    [SerializeField] float speed;

    protected override void onStateEnter()
    {
        print("WALKING entered");
        controls.MoveReleased += OnMoveReleased;
    }


    protected override void onStateExit()
    {
        print("WALKING exited");
        controls.MoveReleased -= OnMoveReleased;
    }

    private void OnMoveReleased()
    {
        setState<TestIdle>();
    }

    protected override void onStateInit()
    {
        return;
    }

    protected override void onStateUpdate()
    {
        return;
    }
    

    protected override void onStateFixedUpdate()
    {
        transform.parent.position += new Vector3(controls.MoveDirection() *speed, 0, 0);
    }

    public override void ResetState()
    {
      
    }
}
