using UnityEngine;

public interface IVelocity2DManager : IVelocity2DProvider
{
    void SetVelocityX(float i_velocityX);

    void SetVelocityY(float i_velocityY);

    void SetVelocity(Vector2 i_velocity);

    void AddVelocityX(float i_velocityX);

    void AddVelocityY(float i_velocityY);
}
