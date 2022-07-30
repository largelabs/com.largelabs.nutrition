using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_fall : State
{
    [SerializeField] PhysicsBody2D body;
    [SerializeField] HarankashControls controller;
    [SerializeField] float fall_gravity;
    [SerializeField] float strife_velocity_fall;
    protected override void onStateEnter()
    {
        Debug.Log("Entered Fall");
        controller.JumpReleased += OnGlide;
        body.SetGravityModifier(fall_gravity);
    }

    protected override void onStateExit()
    {
        Debug.Log("Exited Fall");
        controller.JumpReleased -= OnGlide;
        body.SetGravityModifier(1f);
    }

    protected override void onStateFixedUpdate()
    {
        body.SetVelocityX(strife_velocity_fall * controller.MoveDirection());
        if(true == body.IsGrounded)
        {
            body.SetVelocityX(0);
            setState<S_idle>();
        }
    }

    protected override void onStateInit()
    {
        Debug.Log("Fall State initialized");
        //put neccessary settings and set variables
    }

    protected override void onStateUpdate()
    {
        //implement falling animation and frame swappers here
        return;
    }

    private void OnGlide()
    {
        setState<S_glide>();
    }
}
