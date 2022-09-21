using System;
using UnityEngine;

public class HarankashBounceState : HarankashJumpState
{
    [SerializeField] HarrankashTouchEventDispatcher eventDispatcher = null;

    protected override void onStateEnter()
    {
        if (!body.IsGrounded)
        {
            setState<HarankashFallState>();
            return;
        }

        HaraPlatformAbstract collidedPlatform = getCollidedPlatformComponent();

        if (collidedPlatform == null)
        {
            Debug.LogError("No hara platform component found! setting state to idle.");
            setState<HarankashIdleState>();
            return;
        }

        if (collidedPlatform.GetComponent<OneJumpHaraPlatform>())
            eventDispatcher.DispatchOrangeTouchEvent(collidedPlatform.transform.position);

        maxJumpHeight = collidedPlatform.MaxJumpHeight;
        accelerationData = collidedPlatform.AccelerationConfig;
        collidedPlatform.onCollision();
        base.onStateEnter();
    }

    private HaraPlatformAbstract getCollidedPlatformComponent()
    {
        HaraPlatformAbstract collidedPlatform = body.CurrentGroundTransform.gameObject.GetComponentInParent<HaraPlatformAbstract>();

        if (collidedPlatform == null)
            collidedPlatform = body.CurrentGroundTransform.gameObject.GetComponent<HaraPlatformAbstract>();

        if (collidedPlatform == null)
            collidedPlatform = body.CurrentGroundTransform.gameObject.GetComponentInChildren<HaraPlatformAbstract>();           

        return collidedPlatform;
    }
}
