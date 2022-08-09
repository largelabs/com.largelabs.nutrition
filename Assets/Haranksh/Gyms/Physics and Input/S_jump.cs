using UnityEngine;

public class S_jump : MoveHorizontalAbstractState
{
    [SerializeField] Vector2 jump_speed;
    protected override void onStateEnter()
    {
        Debug.Log("Enter jump");
        base.onStateEnter();
        body.SetVelocity(jump_speed);
    }

    protected override void onStateFixedUpdate()
    {
        base.onStateFixedUpdate();
        if(body.VelocityY <= 0)
        {
            setState<S_fall>();
        }
    }
    protected override void onStateUpdate()
    {
    }
}
