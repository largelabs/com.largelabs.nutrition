using UnityEngine;

[CreateAssetMenu(fileName = "PhysicsConfig2D", menuName = "com.largelabs.nutrition/Physics2D/PhysicsConfig2D", order = 1)]
public class PhysicsConfig2D : ScriptableObject
{
    [Header("Collision")]
    [SerializeField] private bool enableCollisions = true;
    [SerializeField] private RaycastSource raycastSource = RaycastSource.RIGIDBODY;
    [SerializeField] [Range(2, 24)] private int collisionBufferSize = 16;
    [SerializeField] private float minGroundNormalY = 0.65f;
    [SerializeField] private float minMoveDistance = 0.001f;
    [SerializeField] protected float shellRadius = 0.01f;

    [Header("Layer masks")]
    [SerializeField] private LayerMask groundLayerMask = 0;
    [SerializeField] private LayerMask oneWayGroundLayerMask = 0;
    [SerializeField] private LayerMask movingGroundLayerMask = 0;

    [Header("Gravity")]
    [SerializeField] private float gravityModifier = 1f;

    #region PUBLIC API

    public float GravityModifier => gravityModifier;

    public RaycastSource RaycastSource => raycastSource;

    /// <summary>
    /// All allowed ground collision layers
    /// </summary>
    public LayerMask GroundLayerMask => groundLayerMask;

    /// <summary>
    /// Ground layers that will have a one way ground behaviour
    /// </summary>
    public LayerMask OneWayGroundLayerMask => oneWayGroundLayerMask;

    /// <summary>
    /// Ground layers that will be in motion
    /// </summary>
    public LayerMask MovingGroundLayerMask => movingGroundLayerMask;

    /// <summary>
    /// The minimal ground normal value with which the player can be grounded
    /// </summary>
    public float MinGroundNormalY => minGroundNormalY;

    /// <summary>
    /// We only apply collision if the delta move of our object is greater than this minimum distance in a physics frame.
    /// </summary>
    public float MinMoveDistance => minMoveDistance;

    /// <summary>
    /// Distance added to the raycasting, to make sure that colliders don't get stuck into eachother.
    /// </summary>
    public float ShellRadius => shellRadius;

    public bool EnableCollisions => enableCollisions;

    public int CollisionBufferSize => collisionBufferSize;

    #endregion
}
