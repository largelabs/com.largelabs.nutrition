
using UnityEngine;

public abstract class MoveHorizontalAbstractState : State
{
    public PhysicsBody2D body;
    public float fall_speed;
    public float steer_speed;

    protected float originalVelocityX;
    protected override void onStateEnter()
    {
        originalVelocityX = body.VelocityX;
        controls.MoveStarted += OnSteerPressed;
        controls.MoveReleased += OnSteerReleased;
        OnSteerPressed();
    }

    protected override void onStateExit()
    {
        controls.MoveStarted -= OnSteerPressed;
        controls.MoveReleased -= OnSteerReleased;
    }

    protected override void onStateFixedUpdate()
    {
        body.AddVelocityY(-fall_speed);
    }

    protected override void onStateInit()
    {
    }

    protected override void onStateUpdate()
    {
        if (true == body.IsGrounded)
        {
            setState<S_idle>();
            return;
        }
    }
    private void OnSteerPressed()
    {
        body.AddVelocityX(steer_speed * controls.MoveDirection());
    }
    private void OnSteerReleased()
    {
        body.SetVelocityX(originalVelocityX);
    }
}