using UnityEngine;

public class S_jump : MoveHorizontalAbstractState
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
        body.ResetGravityModifier();
    }

    protected override void onStateInit()
    {
        Debug.Log("Jump State initialized");
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
    }
}
