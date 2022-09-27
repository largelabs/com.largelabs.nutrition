
using Cinemachine;
using System.Collections;
using UnityEngine;

public class HarankashFallState : FallAbstractState
{
    [Header("Frames")]
    [SerializeField] private SpriteFrameSwapper fallingFrames = null;
    [SerializeField] private SpriteFrameSwapper landingFrames = null;
    [SerializeField] private SpriteFrameSwapper jumpRiseFrames = null;

    [Header("VFX")]
    [SerializeField] private SpriteFrameSwapper landVFX = null;
    [SerializeField] private AudioSource impactSFX = null;
    [SerializeField] private float timeBeforeBounce = 0.5f;
    [SerializeField] private TrailRenderer trail = null;
    [SerializeField] private InterpolatorsManager interpolatorsManager = null;

    [Header("Camera")]
    [SerializeField] VCamSwitcher vCamSwitcher = null;
    [SerializeField] CinemachineVirtualCamera nearCam = null;

    [Header("Extra Configs")]
    [SerializeField] HarrankashPlatformEventDispatcher eventDispatcher = null;
    [SerializeField] MinigameTimer mgTimer = null;
    [SerializeField] HarankashBounceState bounceState = null;

    Coroutine landingRoutine = null;

    private bool firstFall = true;

    #region PROTECTED
    protected override void onStateInit()
    {

    }

    protected override void onStateEnter()
    {
        jumpRiseFrames.Stop();

        fallingFrames.Play();
        controls.EnableControls();
    }

    protected override void onStateUpdate()
    {
        if (true == body.IsGrounded && body.CurrentGroundTransform != null)
        {
            if (landingRoutine == null)
                landingRoutine = StartCoroutine(landingSequence(body.CurrentGroundTransform.gameObject.tag));
        }
    }

    protected override void onStateExit()
    {
        this.DisposeCoroutine(ref landingRoutine);
    }

    #endregion

    #region PRIVATE
    private IEnumerator landingSequence(string i_tag)
    {
        body.SetVelocityX(0);
        body.SetVelocityY(0);
        controls.DisableControls();

        fallingFrames.Stop();
        landingFrames.Play();
        landVFX.ResetAnimation();
        landVFX.Play();

        HaraPlatformAbstract platform = getCollidedPlatformComponent();
        if (platform != null)
        {
            HarraPlatformAnimationManager animations = platform.GetComponentInChildren<HarraPlatformAnimationManager>();

            if (platform.GetComponent<OneJumpHaraPlatform>())
                eventDispatcher.DispatchOrangeTouchEvent(platform.transform.position);
            else if (animations.IsOpen == false)
                eventDispatcher.DispatchNormalFirstTouchEvent(platform.transform.position);

            if (animations != null)
                animations.OpenUp();

            PlatformID pID = platform.GetComponent<PlatformID>();
            if (pID != null)
                if (pID.PType != PlatformID.PlatformType.Yellow)
                    vCamSwitcher.SwitchToVCam(nearCam);


            //VFX Make platform go down using animation manager
            if (animations != null)
                animations.Nudge(interpolatorsManager);
        }

        // sfx suggestion: impact sound
        impactSFX?.Play();

        yield return this.Wait(timeBeforeBounce);
        landingFrames.Stop();

        if (platform != null)
            platform.onCollision();

        if (i_tag == "Finish" || mgTimer.RemainingTimeSeconds < 0.05f)
        {
            trail.enabled = false;
            yield return this.Wait(0.01f);

            if (firstFall == false)
            {
                firstFall = true;
                eventDispatcher.DispatchFailGameEvent();
            }
            else
                setState<HarankashIdleState>();
        }
        else
        {
            firstFall = false;
            if (i_tag == "Bouncy")
            {
                bounceState.SetFallPlatform(platform);
                setState<HarankashBounceState>();
            }
            else if (i_tag == "Respawn")
            {
                Debug.LogError("ENTER CELEBRATION");
                setState<HarrankashCelebrationState>();

            }
            else
                setState<HarankashIdleState>();
        }

        this.DisposeCoroutine(ref landingRoutine);
    }
    #endregion

    #region UTILITY
    private HaraPlatformAbstract getCollidedPlatformComponent()
    {
        Debug.LogError("Get component fall");
        HaraPlatformAbstract collidedPlatform = body.CurrentGroundTransform.gameObject.GetComponentInParent<HaraPlatformAbstract>();

        if (collidedPlatform == null)
            collidedPlatform = body.CurrentGroundTransform.gameObject.GetComponent<HaraPlatformAbstract>();

        if (collidedPlatform == null)
            collidedPlatform = body.CurrentGroundTransform.gameObject.GetComponentInChildren<HaraPlatformAbstract>();

        return collidedPlatform;
    }
    #endregion
}
