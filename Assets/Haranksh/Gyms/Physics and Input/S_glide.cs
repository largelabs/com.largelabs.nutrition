using UnityEngine;

public class S_glide : MoveHorizontalAbstractState
{
    protected override void onStateEnter()
    {
        Debug.Log("Enter glide");
        base.onStateEnter();
        controls.JumpReleased += OnFall;
        body.SetVelocityY(0);
    }

    protected override void onStateExit()
    {
        base.onStateExit();
        controls.JumpReleased -= OnFall;
    }
    private void OnFall()
    {
        setState<S_fall>();
    }
}
