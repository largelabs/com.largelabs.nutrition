using Cinemachine;
using UnityEngine;

public class HarankashBounceState : HarankashJumpState
{
    [Header("Cameras")]
    [SerializeField] VCamSwitcher vCamSwitcher = null;
    [SerializeField] CinemachineVirtualCamera farCam = null;

    [Header("SFX")]
    [SerializeField] AudioSource bounceSFX = null;

    [Header("Extra Configs")]
    [SerializeField] InterpolatorsManager interpolatorsManager = null;
    [SerializeField] HarrankashPlatformEventDispatcher eventDispatcher = null;
    [SerializeField] MinigameTimer mgTimer = null;

    private HaraPlatformAbstract fallPlatform = null;

    #region STATE API
    protected override void onStateEnter()
    {
        if (!body.IsGrounded)
        {
            setState<HarankashFallState>();
            return;
        }

        HaraPlatformAbstract collidedPlatform = getCollidedPlatformComponent();

        if (collidedPlatform == null)
            return;

        HarraPlatformAnimationManager animations = collidedPlatform.GetComponentInChildren<HarraPlatformAnimationManager>();

        if (collidedPlatform != fallPlatform)
        {
            if (collidedPlatform.GetComponent<OneJumpHaraPlatform>())
                eventDispatcher.DispatchOrangeTouchEvent(collidedPlatform.transform.position);
            else if(animations.IsOpen == false)
                eventDispatcher.DispatchNormalFirstTouchEvent(collidedPlatform.transform.position);

            if (animations != null)
                animations.OpenUp();

            collidedPlatform.onCollision();
        }

        PlatformID pID = collidedPlatform.GetComponent<PlatformID>();
        if (pID != null)
            if (pID.PType == PlatformID.PlatformType.Yellow)
                vCamSwitcher.SwitchToVCam(farCam);

        //VFX Make platform wobble using animation manager
        if (animations != null)
        {
            animations.Wobble2();
            animations.Flutter();
        }

        // sfx suggestion: bouncy jump sound
        bounceSFX?.Play();

        maxJumpHeight = collidedPlatform.MaxJumpHeight;
        accelerationData = collidedPlatform.AccelerationConfig;
        base.onStateEnter();
    }

    public override void ResetState()
    {
        StopAllCoroutines();
        bounceSFX?.Stop();
        onStateExit();
    }
    #endregion

    #region PRIVATE
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
        else if(collidedPlatform == null)
        {
            collidedPlatform = fallPlatform;
            if (collidedPlatform == null)
            {
                Debug.LogError("No hara platform component found! setting state to idle.");
                setState<HarankashIdleState>();
            }
        }

        return collidedPlatform;
    }

    public void SetFallPlatform(HaraPlatformAbstract i_platform)
    {
        fallPlatform = i_platform;
    }
    #endregion
}
