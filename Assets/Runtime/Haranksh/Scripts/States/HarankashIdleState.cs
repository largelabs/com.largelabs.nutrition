using System.Collections;
using UnityEngine;

public class HarankashIdleState : State
{
    [SerializeField] PhysicsBody2D body;
    [SerializeField] SpriteFrameSwapper idlingFrames = null;
    [SerializeField] SpriteFrameSwapper jumpAnticipationFrames = null;
    [SerializeField] Transform visualObjectRoot = null;
    [SerializeField] float visualObjectYOffset = -0.0174f;
    [SerializeField] private TrailRenderer trail = null;
    [SerializeField] private AudioSource jumpSFX = null;

    Coroutine jumpRoutine = null;

    #region PROTECTED
    protected override void onStateInit()
    {
        Debug.Log("Idle State initialized");
    }

    protected override void onStateEnter()
    {
        Debug.Log("Entered IDLE");
        body.SetVelocityX(0f);
        body.SetVelocityY(0f);
        controls.EnableControls();
        controls.JumpPressed += onJump;

        matchVisualToCollider();
        idlingFrames.Play();

        trail.enabled = false;
    }

    protected override void onStateExit()
    {
        Debug.Log("Exited IDLE");
        if (body == null)
        {
            Debug.LogError("NO PHYSICS IN STATES");
            return;
        }
        controls.JumpPressed -= onJump;

        this.DisposeCoroutine(ref jumpRoutine);
    }

    protected override void onStateUpdate()
    {
        checkFall();
    }

    public override void ResetState()
    {
        StopAllCoroutines();
        idlingFrames.Stop();
        idlingFrames.ResetAnimation();
        jumpAnticipationFrames.Stop();
        jumpAnticipationFrames.ResetAnimation();
        jumpSFX?.Stop();
        onStateExit();
    }

    #endregion

    #region PRIVATE

    private void onJump()
    {
        // sfx suggestion: launch/strong jump sound
        jumpSFX?.Play();

        if (jumpRoutine == null)
        {
            jumpRoutine = StartCoroutine(onJumpSequence());
            controls.JumpPressed -= onJump;
        }
    }

    private IEnumerator onJumpSequence()
    {
        idlingFrames.Stop();
        jumpAnticipationFrames.Play();
        yield return this.Wait(0.3f);
        jumpAnticipationFrames.Stop();

        setState<HarankashJumpState>();

        this.DisposeCoroutine(ref jumpRoutine);
    }

    private void matchVisualToCollider()
    {
        Vector3 temp = visualObjectRoot.localPosition;
        visualObjectRoot.localPosition = new Vector3(temp.x, visualObjectYOffset, temp.z);
    }

    void checkFall()
    {
        if (false == enabled) return;

        if (false == body.IsGrounded && body.VelocityY < -0.1f)
        {
            setState<HarankashFallState>();
        }
    }

    #endregion
}
