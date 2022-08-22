using UnityEngine;

public class S_bounce : S_jump
{
    protected override void onStateEnter()
    {
        if (!body.IsGrounded)
        {
            setState<S_fall>();
            return;
        }
        HaraPlatformAbstract ground = body.CurrentGroundTransform.gameObject.GetComponent<HaraPlatformAbstract>();

        if (ground == null)
        {
            setState<S_idle>();
            return;
        }
        maxJumpHeight = ground.MaxJumpHeight;
        accelerationData = ground.AccelerationConfig;
        ground.onCollision();
        base.onStateEnter();
        
    }
}
