
using UnityEngine;

public interface ICollisions2DConfig
{
    /// <summary>
    /// PhysicsBody2Ds can cast from either the whole rigidbody shape or a given collider
    /// </summary>
    RaycastSource RaycastSource { get; }

    /// <summary>
    /// The minimal ground normal value with which the player can be grounded
    /// </summary>
    float MinGroundNormalY { get; }

    /// <summary>
    /// We only apply collision if the delta move of our object is greater than this minimum distance in a physics frame.
    /// </summary>
    float MinMoveDistance { get; }

    /// <summary>
    /// Distance added to the raycasting, to make sure that colliders don't get stuck into eachother.
    /// </summary>
    float ShellRadius { get; }

    /// <summary>
    /// The size of the result array for raycasting. The array cannot be resized at runtime.
    /// </summary>
    int CollisionBufferSize { get; }

    /// <summary>
    /// All allowed ground collision layers
    /// </summary>
    LayerMask GroundLayerMask { get; }

    /// <summary>
    /// Ground layers that will have a one way ground behaviour
    /// </summary>
    LayerMask OneWayGroundLayerMask { get; }

    /// <summary>
    /// Ground layers that will be in motion. WIP
    /// </summary>
   // LayerMask MovingGroundLayerMask { get; }

    /// <summary>
    /// Whether or not collisions are enabled for this object
    /// </summary>
    bool IsCollisionEnabled { get; }
}
