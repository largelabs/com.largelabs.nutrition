using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_jump : State
{
    [SerializeField] PhysicsBody2D body;
    [SerializeField] Vector2 Jump_speed;
    [SerializeField] float Fall_speed;
    protected override void onStateEnter()
    {
        Debug.Log("Entered Jump");
        body.SetVelocity(Jump_speed);
        body.SetGravityModifier(Fall_speed);
    }

    protected override void onStateExit()
    {
        return;
        body.ResetGravityModifier();
    }

    protected override void onStateInit()
    {
        Debug.Log("Jump State initialized");
        //put neccessary settings and set variables
    }

    protected override void onStateFixedUpdate()
    {
        if(body.VelocityY <= 0)
        {
            setState<S_glide>();
        }
    }
    protected override void onStateUpdate()
    {
        return;
    }
}
