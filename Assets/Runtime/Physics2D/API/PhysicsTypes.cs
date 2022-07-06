public enum RaycastSource
{
    RIGIDBODY = 0,
    COLLIDER = 1
};

public static class Physics2DConstants
{
    public static readonly float EPSILON_VELOCITY = 0.0001f;
}

public delegate void Physics2DWallEvent(IWallDetector i_wallDetector);

public delegate void Physics2DGroundEvent(IGroundedObject2D i_groundedObject);