using UnityEngine;

public class S_glide : MoveHorizontalAbstractState
{
    [SerializeField] PhysicsBody2D body;
    [SerializeField] HarankashControls controller;
    [SerializeField] float glide_gravity;
    [SerializeField] float strife_velocity_glide;

    private float originalVelocityX;
    protected override void onStateEnter()
    {
        Debug.Log("Entered Glide");
        originalVelocityX = body.VelocityX;
        controller.JumpPressed += OnFall;
        controller.MoveStarted += OnSteerPressed;
        controller.MoveReleased += OnSteerReleased;
        OnSteerPressed();
        body.SetGravityModifier(0);
        body.SetVelocityY(0);
    }

    protected override void onStateExit()
    {
        Debug.Log("Exited Glide");
        controller.JumpPressed -= OnFall;
        controller.MoveStarted -= OnSteerPressed;
        controller.MoveReleased -= OnSteerReleased;
        OnSteerReleased();
        body.ResetGravityModifier();
    }

    protected override void onStateFixedUpdate()
    {
        if (true == body.IsGrounded)
        {
            setState<S_idle>();
            return;
        }
        body.SetVelocityY(-glide_gravity);

    }

    protected override void onStateInit()
    {
        Debug.Log("Glide State initialized");
        //put neccessary settings and set variables
    }

    protected override void onStateUpdate()
    {

    }
    private void OnFall()
    {
        setState<S_fall>();
    }
    
    private void OnSteerPressed()
    {
        body.AddVelocityX(strife_velocity_glide * controller.MoveDirection());
    }
    private void OnSteerReleased()
    {
        body.SetVelocityX(originalVelocityX);
    }
}
