using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_idle : State
{
    [SerializeField] HarankashControls controller;
    [SerializeField] PhysicsBody2D body;
    [SerializeField] Vector2 jump_speed;
    protected override void onStateEnter()
    {
        Debug.Log("Entered IDLE");
        controller.JumpPressed += OnJump;
        //play appropriate animation and sounds
        return;
    }

    protected override void onStateExit()
    {
        Debug.Log("Exited IDLE");
        if (body == null)
        {
            Debug.LogError("NO PHYSICS IN STATES");
            return;
        }
        body.SetVelocity(jump_speed);

        controller.JumpPressed -= OnJump;

        //Play Appropriate sound and animation
        return;
    }

    protected override void onStateInit()
    {
        Debug.Log("Idle State initialized");
        //put neccessary settings and set variables
    }

    protected override void onStateUpdate()
    {
        //just idling, play animation and sound here
        return;
    }
    private void OnJump(){
        setState<S_jump>();
    }
}
