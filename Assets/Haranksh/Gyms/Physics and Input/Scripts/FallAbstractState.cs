
using UnityEngine;

public abstract class FallAbstractState : MoveHorizontalAbstractState
{
    #region PROTECTED

    protected override void onStateUpdate()
    {
        if (true == body.IsGrounded)
        {
            if(body.CurrentGroundTransform.gameObject.tag == "Bouncy")
            {
                setState<HarankashBounceState>();
                return;
            }
            setState<HarankashIdleState>();
            return;
        }
    }

    protected override void onStateFixedUpdate()
    {
        base.onStateFixedUpdate();
        updateFallVelocity();
    }

    #endregion

    #region PRIVATE

    void updateFallVelocity()
    {
        if (false == enabled) return;

        float addedVelocityY = body.GravityVector.y * (accelerationData.AccelerationY - 1) * Time.fixedDeltaTime;
        body.AddVelocityY(addedVelocityY);
    }

    #endregion

}
