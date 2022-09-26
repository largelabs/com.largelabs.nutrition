using System;
using UnityEngine;

public class HarankashBounceState : HarankashJumpState
{
    [SerializeField] HarrankashPlatformEventDispatcher eventDispatcher = null;
    [SerializeField] MinigameTimer mgTimer = null;

    private HaraPlatformAbstract fallPlatform = null;

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

        if (collidedPlatform != fallPlatform)
        {
            HarraPlatformAnimationManager animations = collidedPlatform.GetComponentInChildren<HarraPlatformAnimationManager>();
            if (animations != null)
                animations.OpenUp();

            if (collidedPlatform.GetComponent<OneJumpHaraPlatform>())
                eventDispatcher.DispatchOrangeTouchEvent(collidedPlatform.transform.position);

            collidedPlatform.onCollision();
        }

        // sfx suggestion: bouncy jump sound

        maxJumpHeight = collidedPlatform.MaxJumpHeight;
        accelerationData = collidedPlatform.AccelerationConfig;
        base.onStateEnter();
    }

    private HaraPlatformAbstract getCollidedPlatformComponent()
    {
        Debug.LogError("Get component bounce");

        HaraPlatformAbstract collidedPlatform = body.CurrentGroundTransform.gameObject.GetComponentInParent<HaraPlatformAbstract>();

        if (collidedPlatform == null)
            collidedPlatform = body.CurrentGroundTransform.gameObject.GetComponent<HaraPlatformAbstract>();

        if (collidedPlatform == null)
            collidedPlatform = body.CurrentGroundTransform.gameObject.GetComponentInChildren<HaraPlatformAbstract>();

        if (body.CurrentGroundTransform.gameObject.tag == "Finish" || mgTimer.RemainingTimeSeconds < 0.05f)
        {
            trail.enabled = false;

            eventDispatcher.DispatchFailGameEvent();
        }

        return collidedPlatform;
    }

    public void SetFallPlatform(HaraPlatformAbstract i_platform)
    {
        fallPlatform = i_platform;
    }
}
