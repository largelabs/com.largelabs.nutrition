
using UnityEngine;

public abstract class FallAbstractState : MoveHorizontalAbstractState
{
    #region PROTECTED

    protected override void onStateUpdate()
    {
        if (true == body.IsGrounded)
        {
            setState<S_idle>();
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
