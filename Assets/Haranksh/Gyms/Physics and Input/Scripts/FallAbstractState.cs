
using System.Collections;
using UnityEngine;

public abstract class FallAbstractState : MoveHorizontalAbstractState
{
    [SerializeField] private SpriteFrameSwapper fallingFrames = null;
    [SerializeField] private SpriteFrameSwapper landingFrames = null;
    [SerializeField] private float timeBeforeBounce = 0.5f;

    Coroutine landingRoutine = null;

    #region PROTECTED
    protected override void onStateEnter()
    {
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
        controls.DisableControls();

        fallingFrames.Stop();
        fallingFrames.ResetAnimation();
        landingFrames.Play();
        yield return this.Wait(timeBeforeBounce);
        landingFrames.Stop();
        landingFrames.ResetAnimation();

        controls.EnableControls();
        if (i_tag == "Bouncy")
            setState<HarankashBounceState>();
        else
            setState<HarankashIdleState>();

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
