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

    public LayerMask GroundLayerMask => groundLayerMask;

    public LayerMask OneWayGroundLayerMask => oneWayGroundLayerMask;

    public LayerMask MovingGroundLayerMask => movingGroundLayerMask;

    public float MinGroundNormalY => minGroundNormalY;

    public float MinMoveDistance => minMoveDistance;

    public float ShellRadius => shellRadius;

    public bool EnableCollisions => enableCollisions;

    public int CollisionBufferSize => collisionBufferSize;

    #endregion
}
