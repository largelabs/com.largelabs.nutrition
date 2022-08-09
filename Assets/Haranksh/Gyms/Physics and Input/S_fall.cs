using UnityEngine;

public class S_fall : MoveHorizontalAbstractState
{
    protected override void onStateEnter()
    {
        Debug.Log("Enter fall");
        base.onStateEnter();
        controls.JumpPressed += OnGlide;
    }

    protected override void onStateExit()
    {
        base.onStateExit();
        controls.JumpPressed -= OnGlide;
    }

    private void OnGlide()
    {
        setState<S_glide>();
    }
}
