
using UnityEngine;

public interface IVelocity2DProvider
{
    float VelocityX { get; }

    float VelocityY { get; }

    Vector2 Velocity2D { get; }
}