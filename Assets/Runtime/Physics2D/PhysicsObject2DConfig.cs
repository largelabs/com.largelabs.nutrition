using UnityEngine;

[CreateAssetMenu(fileName = "PhysicsObject2DConfig", menuName = "com.largelabs.nutrition/Physics2D/PhysicsObject2DConfig", order = 1)]
public class PhysicsObject2DConfig : ScriptableObject, IGravity2DConfig, ICollisions2DConfig
{
    [Header("Collision")]
    [SerializeField] private bool enableCollisions = true;
    [SerializeField] private RaycastSource raycastSource = RaycastSource.RIGIDBODY;
    [SerializeField] [Range(2, 24)] private int collisionBufferSize = 16;
    [SerializeField] private float minGroundNormalY = 0.65f;
    [SerializeField] private float minMoveDistance = 0.001f;
    [SerializeField] private float shellRadius = 0.01f;

    [Header("Layer masks")]
    [SerializeField] private LayerMask groundLayerMask = 0;
    [SerializeField] private LayerMask oneWayGroundLayerMask = 0;
   // [SerializeField] private LayerMask movingGroundLayerMask = 0; // WIP

    [Header("Gravity")]
    [SerializeField] private float gravityModifier = 1f;

    [System.NonSerialized] private bool enableCollisions_init = true;
    [System.NonSerialized] private RaycastSource raycastSource_init = RaycastSource.RIGIDBODY;
    [System.NonSerialized] private int collisionBufferSize_init = 16;
    [System.NonSerialized] private float minGroundNormalY_init = 0.65f;
    [System.NonSerialized] private float minMoveDistance_init = 0.001f;
    [System.NonSerialized] private float shellRadius_init = 0.01f;
    [System.NonSerialized] private LayerMask groundLayerMask_init = 0;
    [System.NonSerialized] private LayerMask oneWayGroundLayerMask_init = 0;
    [System.NonSerialized] private LayerMask movingGroundLayerMask_init = 0;
    [System.NonSerialized] private float gravityModifier_init = 1f;

    #region UNITY AND CORE

    private void OnEnable()
    {
        setInitialValues();
    }

    #endregion

    #region MUTABLE API

    public void ResetToInitialState() { resetToInitialValues(); }

    public void SetGravityModifier(float i_gravityModifier) { gravityModifier = i_gravityModifier; }

    public void ResetGravityModifier() { gravityModifier = gravityModifier_init; }

    #endregion

    #region ICollisions2DConfig

    public bool IsCollisionEnabled => enableCollisions;

    public LayerMask GroundLayerMask => groundLayerMask;

    public LayerMask OneWayGroundLayerMask => oneWayGroundLayerMask;

   // public LayerMask MovingGroundLayerMask => movingGroundLayerMask; // WIP

    public RaycastSource RaycastSource => raycastSource;

    public float MinGroundNormalY => minGroundNormalY;

    public float MinMoveDistance => minMoveDistance;

    public float ShellRadius => shellRadius;

    public int CollisionBufferSize => collisionBufferSize;

    #endregion

    #region IGravity2DConfig

    public float GravityModifier => gravityModifier;

    public Vector2 GravityVector => Physics2D.gravity * gravityModifier;

    #endregion

    #region PRIVATE

    void setInitialValues()
    {
        enableCollisions_init = enableCollisions;
        raycastSource_init = raycastSource;
        collisionBufferSize_init = collisionBufferSize;
        minGroundNormalY_init = minGroundNormalY;
        minMoveDistance_init = minMoveDistance;
        shellRadius_init = shellRadius;
        groundLayerMask_init = groundLayerMask;
        oneWayGroundLayerMask_init = oneWayGroundLayerMask;
      //  movingGroundLayerMask_init = movingGroundLayerMask;
        gravityModifier_init = gravityModifier;
    }

    void resetToInitialValues()
    {
        enableCollisions = enableCollisions_init;
        raycastSource = raycastSource_init;
        collisionBufferSize = collisionBufferSize_init;
        minGroundNormalY = minGroundNormalY_init;
        minMoveDistance = minMoveDistance_init;
        shellRadius = shellRadius_init;
        groundLayerMask = groundLayerMask_init;
        oneWayGroundLayerMask = oneWayGroundLayerMask_init;
      //  movingGroundLayerMask = movingGroundLayerMask_init;
        gravityModifier = gravityModifier_init;
    }

    #endregion
}
