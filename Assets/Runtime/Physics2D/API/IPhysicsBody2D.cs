public interface IPhysicsBody2D
{
    IGravity2DConfig GravityConfig { get; }

    ICollisions2DConfig CollisionsConfig { get; }
}
