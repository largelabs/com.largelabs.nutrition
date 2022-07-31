using UnityEngine;

public class S_fall : MoveHorizontalAbstractState
{
    [SerializeField] PhysicsBody2D body;
    [SerializeField] HarankashControls controller;
    [SerializeField] float fall_gravity;
    [SerializeField] float strife_velocity_fall;

    private float originalVelocityX;
    protected override void onStateEnter()
    {
        Debug.Log("Entered Fall");
        originalVelocityX = body.VelocityX;
        controller.JumpReleased += OnGlide;
        controller.MoveStarted += OnSteerPressed;
        controller.MoveReleased += OnSteerReleased;
        OnSteerPressed();
        body.SetGravityModifier(0);
    }

    protected override void onStateExit()
    {
        Debug.Log("Exited Fall");
        controller.JumpReleased -= OnGlide;
        controller.MoveStarted -= OnSteerPressed;
        controller.MoveReleased -= OnSteerReleased;
        OnSteerReleased();
        body.ResetGravityModifier();
    }

    protected override void onStateFixedUpdate()
    {
        if(true == body.IsGrounded)
        {
            setState<S_idle>();
            return;
        }
        body.SetVelocityY(-fall_gravity);
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

    private void OnSteerPressed()
    {
        body.AddVelocityX(strife_velocity_fall * controller.MoveDirection());
    }
    private void OnSteerReleased()
    {
        body.SetVelocityX(originalVelocityX);
    }
}
