using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_glide : State
{
    [SerializeField] PhysicsBody2D body;
    [SerializeField] HarankashControls controller;
    [SerializeField] float glide_gravity;
    [SerializeField] float strife_velocity_glide;
    protected override void onStateEnter()
    {
        Debug.Log("Entered Glide");
        controller.JumpPressed += OnFall;
        body.SetGravityModifier(glide_gravity);
        body.SetVelocity(new Vector2(0, 0));
    }

    protected override void onStateExit()
    {
        Debug.Log("Exited Glide");
        controller.JumpPressed -= OnFall;
        body.SetGravityModifier(1f);
    }

    protected override void onStateFixedUpdate()
    {
        body.SetVelocityX(strife_velocity_glide * controller.MoveDirection());
        if (true == body.IsGrounded)
        {
            body.SetVelocityX(0);
            setState<S_idle>();
        }
    }

    protected override void onStateInit()
    {
        Debug.Log("Glide State initialized");
        //put neccessary settings and set variables
    }

    protected override void onStateUpdate()
    {
        //Implement animations here
        return;
    }
    private void OnFall()
    {
        setState<S_fall>();
    }
}
