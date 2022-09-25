
using System.Collections;
using UnityEngine;

public abstract class FallAbstractState : MoveHorizontalAbstractState
{
    [SerializeField] private SpriteFrameSwapper fallingFrames = null;
    [SerializeField] private SpriteFrameSwapper landingFrames = null;
    [SerializeField] private SpriteFrameSwapper jumpRiseFrames = null;
    [SerializeField] private SpriteFrameSwapper landVFX = null;
    [SerializeField] private float timeBeforeBounce = 0.5f;
    [SerializeField] private TrailRenderer trail = null;
    [SerializeField] HarrankashTouchEventDispatcher eventDispatcher = null;

    Coroutine landingRoutine = null;

    private bool firstFall = true;

    #region PROTECTED
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

    protected override void onStateFixedUpdate()
    {
        base.onStateFixedUpdate();
        updateFallVelocity();
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
        // sfx suggestion: impact sound
        yield return this.Wait(timeBeforeBounce);
        landingFrames.Stop();

        if (i_tag == "Finish")
        {
            trail.enabled = false;
            yield return this.Wait(0.01f);

            if (firstFall == false)
            {
                firstFall = true;
                eventDispatcher.DispatchCartTouchEvent();
            }
            else
                setState<HarankashIdleState>();
        }
        else
        {
            firstFall = false;
            if (i_tag == "Bouncy")
                setState<HarankashBounceState>();
            else if (i_tag == "Respawn")
                setState<HarrankashCelebrationState>();
            else
                setState<HarankashIdleState>();
        }

        this.DisposeCoroutine(ref landingRoutine);
    }

    void updateFallVelocity()
    {
        if (false == enabled) return;

        float addedVelocityY = body.GravityVector.y * (accelerationData.AccelerationY - 1) * Time.fixedDeltaTime;
        body.AddVelocityY(addedVelocityY);
    }

    #endregion

}
