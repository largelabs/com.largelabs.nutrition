using UnityEngine;

public class HarankashJumpState : MoveHorizontalAbstractState
{
    [SerializeField][Range(1f, 30f)] protected float maxJumpHeight = 8f;

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

        body.SetVelocityY(accelerationData.MaxVelocityY);

        controls.JumpPressed += goToFastFall;
    }

    protected override void onStateExit()
    {
        controls.JumpPressed -= goToFastFall;
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
