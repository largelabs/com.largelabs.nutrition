
using System.Collections;
using UnityEngine;

public abstract class FallAbstractState : MoveHorizontalAbstractState
{
    protected override void onStateFixedUpdate()
    {
        base.onStateFixedUpdate();
        updateFallVelocity();
    }

    void updateFallVelocity()
    {
        if (false == enabled) return;

        float addedVelocityY = body.GravityVector.y * (accelerationData.AccelerationY - 1) * Time.fixedDeltaTime;
        body.AddVelocityY(addedVelocityY);
    }
}
