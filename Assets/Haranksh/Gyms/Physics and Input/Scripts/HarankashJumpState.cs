using System.Collections;
using UnityEngine;

public class HarankashJumpState : MoveHorizontalAbstractState
{
    [SerializeField][Range(1f, 30f)] protected float maxJumpHeight = 8f;
    [SerializeField] private SpriteFrameSwapper jumpLaunchFrames = null;
    [SerializeField] private SpriteFrameSwapper jumpRiseFrames = null;
    [SerializeField] Transform visualObjectRoot = null;
    [SerializeField] float visualObjectYOffset = 0.1263f;

    Coroutine launchRoutine = null;

    float startJumpY = 0f;
    float stopJumpY = 0f;

    #region PROTECTED
    protected override void onStateInit()
    {
    }

    protected override void onStateEnter()
    {
        Debug.Log("Enter jump");

        startJumpY = body.transform.position.y;
        stopJumpY = startJumpY + maxJumpHeight;

        Debug.Log(startJumpY + "  " + stopJumpY);

        //controls.JumpPressed += goToFastFall;

        if (launchRoutine == null)
            launchRoutine = StartCoroutine(launchSequence());
        else
            Debug.LogError("Launch Routine already running!");
    }

    protected override void onStateExit()
    {
        controls.JumpPressed -= goToFastFall;
        jumpRiseFrames.Stop();
        jumpRiseFrames.ResetAnimation();
        this.DisposeCoroutine(ref launchRoutine);
    }

    protected override void onStateUpdate()
    {
    }

    protected override void onStateFixedUpdate()
    {
        base.onStateFixedUpdate();
        checkHeight();
    }

    #endregion

    #region PRIVATE
    private IEnumerator launchSequence()
    {
        matchVisualToCollider();

        jumpLaunchFrames.Play();
        yield return this.Wait(0.1f);

        body.SetVelocityY(accelerationData.MaxVelocityY);
        yield return this.Wait(0.3f);

        jumpLaunchFrames.Stop();
        jumpLaunchFrames.ResetAnimation();
        jumpRiseFrames.Play();

        this.DisposeCoroutine(ref launchRoutine);
    }

    private void matchVisualToCollider()
    {
        Vector3 temp = visualObjectRoot.localPosition;
        visualObjectRoot.localPosition = new Vector3(temp.x, visualObjectYOffset, temp.z);
    }

    void checkHeight()
    {
        if (false == enabled) return;

        if (body.transform.position.y >= stopJumpY)
        {
            setState<HarankashFallState>();
            return;
        }

        if(body.VelocityY < 0)
        {
            setState<HarankashFallState>();
            return;
        }
    }

    void goToFastFall()
    {
        setState<HarankashFastFallState>();
    }

    #endregion
}
