using UnityEngine;

public abstract class MoveHorizontalAbstractState : State
{
    [SerializeField] protected AccelerationConfig2D accelerationData = null;
    [SerializeField] protected PhysicsBody2D body = null;

    #region PROTECTED

    protected override void onStateFixedUpdate()
    {
        base.onStateFixedUpdate();
        updateMoveVelocity();
    }

    protected virtual void accelerateX(IAccelerationConfig i_currentAccelerationConfig, float i_velocityX, float i_deltaX)
    {
        float maxVelX = i_currentAccelerationConfig.MaxVelocityX;
        if (Mathf.Abs(i_velocityX) >= maxVelX)
        {
            descelerateX(i_currentAccelerationConfig, body.VelocityX);
        }
        else
        {
            float addedVelocity = i_deltaX * i_currentAccelerationConfig.AccelerationX;
            body.AddVelocityX(addedVelocity);
            clampVelocityX(body.VelocityX, maxVelX);
        }
    }

    protected virtual void descelerateX(IAccelerationConfig i_currentAccelerationConfig, float i_velocityX)
    {
        if (false == enabled) return;

        if (null != i_currentAccelerationConfig && i_velocityX != 0f)
        {
            float velocitySign = Mathf.Sign(i_velocityX);
            float addedVelocity = -velocitySign * i_currentAccelerationConfig.DescelerationX * Time.fixedDeltaTime;

            body.AddVelocityX(addedVelocity);

            if (velocitySign != Mathf.Sign(i_velocityX)) onDidStop();
        }
        else
        {
            onDidStop();
        }
    }

    protected virtual void onDidStop()
    { }

    #endregion

    #region PRIVATE

    void clampVelocityX(float i_velocityX, float i_maxVelocityX)
    {
        if (Mathf.Abs(i_velocityX) >= i_maxVelocityX)
        {
            body.SetVelocityX(Mathf.Sign(i_velocityX) * i_maxVelocityX);
        }
    }

    void updateMoveVelocity()
    {
        float moveDirection = controls.MoveDirection();

        if (moveDirection != 0)
        {
            float deltaX = moveDirection * Time.fixedDeltaTime;
            accelerateX(accelerationData, body.VelocityX, deltaX);
        }
        else
        {
            descelerateX(accelerationData, body.VelocityX);
        }
    }

    #endregion

    public override void ResetState()
    {

    }
}