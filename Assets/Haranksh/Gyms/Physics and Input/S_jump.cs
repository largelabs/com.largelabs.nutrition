using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_jump : State
{
    [SerializeField] PhysicsBody2D body;
    protected override void onStateEnter()
    {
        Debug.Log("Entered Jump");
    }

    protected override void onStateExit()
    {
        return;
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
