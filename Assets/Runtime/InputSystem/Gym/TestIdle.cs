using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestIdle : State
{
    [SerializeField] HarankashControls controls;

    protected override void onStateEnter()
    {
        print("IDLE entered");
        controls.MoveStarted += OnMove;
        controls.JumpPressed += OnJump;
    }

    protected override void onStateExit()
    {
        print("IDLE exited");
        controls.MoveStarted -= OnMove;
        controls.JumpPressed -= OnJump;
    }

    private void OnMove()
    {
        setState<TestWalking>();
    }

    private void OnJump()
    {
        setState<TestJump>();
    }

    protected override void onStateInit()
    {
        return;
    }

    

    protected override void onStateUpdate()
    {
        return;
    }
}
