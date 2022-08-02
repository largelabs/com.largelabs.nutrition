using System;
using System.Collections;
using UnityEngine;


public class TestJump : State
{
    protected override void onStateEnter()
    {
        print("JUMP entered");
        controls.JumpReleased += OnJumpReleased;
        controls.JumpPressed += OnJumpPressed;
    }

    private void OnJumpPressed()
    {
        print("Gliding");
    }

    private void OnJumpReleased()
    {
        print("Falling");
    }

    protected override void onStateExit()
    {
        print("JUMP exited");
        controls.JumpReleased -= OnJumpReleased;
        controls.JumpPressed -= OnJumpPressed;
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
