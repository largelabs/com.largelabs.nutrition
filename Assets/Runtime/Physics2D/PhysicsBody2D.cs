using System.Collections.Generic;
using UnityEngine;

public class PhysicsBody2D : MonoBehaviourBase, IPhysicsBody2D, IWallDetector2D, IGroundedObject2D, IVelocity2DManager
{
    [Header("Gizmos")]
    [SerializeField] GizmoDrawMode gizmoDrawMode = GizmoDrawMode.NONE;
    [SerializeField] Color groundTangentColor = Color.red;
    [SerializeField] Color groundNormalColor = Color.green;
    [SerializeField] Color velocityColor = Color.blue;
    [SerializeField] bool normalizeVelocityGizmo = true;
    [SerializeField] [Range(0.1f, 3f)] float gizmoThickness = 2f;

    [Header("Physics components")]
    [SerializeField] private Collider2D objectCollider = null;
    [SerializeField] private Rigidbody2D objectRgbd2D = null;
    [SerializeField] private PhysicsObject2DConfig physicsConfig = null;

    // Contact filter allows us to filter our raycasting depending on layers for example
    private ContactFilter2D contactFilter = new ContactFilter2D();

    // Our huit buffer will be updated by Unity's casting
    private RaycastHit2D[] hitBuffer = null;

    // These lists are used to cache the successful hits on ither x or y movement during a physics loop.
    private List<RaycastHit2D> hitBufferListX = null;
    private List<RaycastHit2D> hitBufferListY = null;

    // We cache the ground transforms 
    private Dictionary<Transform, RaycastHit2D> groundTransformsBuffer = null;

    // Object transfrom
    private Transform objectTransform = null;
    private Transform initialParent = null;

    // Runtime values
    private Vector2 targetVelocity = MathConstants.VECTOR_2_ZERO; // The inputed velocity 
    private Vector2 velocity = MathConstants.VECTOR_2_ZERO; // The internal velocity updated by the physics system
    private bool isGrounded = false;
    private bool wasGrounded = false;
    private bool isHittingWall = false;
    private bool wasHittingWall = false;
    private Vector2? currentGroundHit = null;
    private Vector2? lastGroundEnterPosition = null;
    private Transform currentGroundTransform = null;
    private int? currentGroundLayer = null;
    private Vector2 groundNormal = MathConstants.VECTOR_2_ZERO;
    private Ground2D currentGroundCmpt = null;
    private Ground2D previousGroundCmpt = null;

    #region UNITY AND CORE

    protected override void Awake()
    {
        base.Awake();


        if (null != physicsConfig) physicsConfig = Instantiate<PhysicsObject2DConfig>(physicsConfig);
        if(null == objectRgbd2D) objectRgbd2D = GetComponent<Rigidbody2D>();
        if (null == objectCollider) objectCollider = GetComponent<Collider2D>();

        initBody();
    }

    private void FixedUpdate()
    {
        if (null == hitBuffer) return;

        wasGrounded = isGrounded;
        wasHittingWall = isHittingWall;

        // Our object is constantly attracted by the gravity, scaled by our gravity modifier
        velocity += physicsConfig.GravityVector * Time.fixedDeltaTime;
        isGrounded = false;

        velocity.x = targetVelocity.x;
        isHittingWall = false;

        // Our position delta at the current physics frame (current velocity * dt). This will be used for our movement.
        Vector2 deltaPosition = velocity * Time.fixedDeltaTime;

        // To make it easier to deal with slopes, we call updateMovement twice : 
        // once for x, once for y, with different parameters involved...

        // Horizontal movement takes a vector that includes the ground normal, 
        // so our entity can move along the ground's tangent
        Vector2 groundTangent = new Vector2(groundNormal.y, -groundNormal.x);
        Vector2 xMov = updateMovement(groundTangent * deltaPosition.x, false);
        objectRgbd2D.position += new Vector2(xMov.x, xMov.y);

        // Vertical movement takes the up vector scaled with our deltaPosition.
        Vector2 yMov = updateMovement(MathConstants.VECTOR_2_UP * deltaPosition.y, true);
        objectRgbd2D.position += new Vector2(yMov.x, yMov.y);

        triggerWallEvents();
        triggerGroundedEvents();

        Debug.LogError("grounded: " + isGrounded);
    }

    #endregion

    #region IPhysicsBody2D
    public IGravity2DConfig GravityConfig => physicsConfig;

    public ICollisions2DConfig CollisionsConfig => physicsConfig;

    public Vector2 GravityVector => (null == physicsConfig ? Physics2D.gravity : physicsConfig.GravityVector);

    #endregion

    #region IVelocity2DManager / IVelocity2DProvider

    [ExposePublicMethod]
    public void SetVelocityX(float i_velocityX)
    {
        targetVelocity.x = i_velocityX;
    }

    [ExposePublicMethod]
    public void SetVelocityY(float i_velocityY)
    {
        velocity.y = i_velocityY;
    }

    [ExposePublicMethod]
    public void SetVelocity(Vector2 i_velocity)
    {
        SetVelocityX(i_velocity.x);
        SetVelocityY(i_velocity.y);
    }

    [ExposePublicMethod]
    public void AddVelocityX(float i_velocityX)
    {
        targetVelocity.x += i_velocityX;
    }

    [ExposePublicMethod]
    public void AddVelocityY(float i_velocityY)
    {
        velocity.y += i_velocityY;
    }

    public float VelocityX => targetVelocity.x;

    public float VelocityY => velocity.y;

    public Vector2 Velocity2D => velocity;

    #endregion

    #region IWallDetector

    public Physics2DWallEvent OnWallStatusChanged { get; set; }

    public bool IsHittingWall => isHittingWall;

    #endregion

    #region IGroundedObject2D

    public bool IsGrounded => isGrounded;

    public Transform CurrentGroundTransform => currentGroundTransform;

    public int? CurrentGroundLayer => currentGroundLayer;

    public Vector2? CurrentGroundHit => currentGroundHit;

    public Vector2? LastGroundEnterPosition => lastGroundEnterPosition;

    public Physics2DGroundEvent OnGroundedStatusChanged { get; set; }

    public Physics2DGroundEvent OnDidUpdateCurrentGroundTransform { get; set; }

    public Physics2DGroundEvent OnDidUpdateCurrentGroundData { get; set; }

    #endregion

    #region MUTABLE API

    [ExposePublicMethod]
    public void SetGravityModifier(float i_gravityModifier) { if (null != physicsConfig) physicsConfig.SetGravityModifier(i_gravityModifier); }

    [ExposePublicMethod]
    public void ResetGravityModifier() { if (null != physicsConfig) physicsConfig.ResetGravityModifier(); }

    [ExposePublicMethod]
    public void ResetToInitialState()
    {
        resetValues();
        if (null != physicsConfig) physicsConfig.ResetToInitialState();
    }

    #endregion

    #region PROTECTED VIRTUAL

    protected virtual bool canCheckForCollision(ref Vector2 i_move, bool i_yMovement, RaycastHit2D i_hit)
    {
        int layer = i_hit.collider.gameObject.layer;
        LayerMask oneWayGroundLayers = physicsConfig.OneWayGroundLayerMask; 
        bool isOneWay = oneWayGroundLayers == (oneWayGroundLayers | (1 << layer));
        bool checkForCollsions = true;
        float shellRadius = physicsConfig.ShellRadius;

        if (true == isOneWay)
        {
            if (i_yMovement)
            {
                if (VelocityY > 0.1f) return false;
                if(null == objectCollider)
                {
                    Debug.LogWarning("PhysicsBody2D::canCheckForCollision -> cannot check one way ground collisions if collider isn't assigned.");
                    return false;
                }

                float hitY = i_hit.point.y;
                float boundsY = objectCollider.bounds.min.y;

                if (hitY /*+ shellRadius*/ > boundsY)
                    checkForCollsions = false;
            }
            else
            {
                i_move.y = 0f;
                checkForCollsions = false;
            }
        }

        return checkForCollsions;
    }

    protected virtual void onWallHitEnter() { }

    protected virtual void onWallHitExit() { }

    protected virtual void onExitGround()
    {
    }

    protected virtual void onEnterGround()
    {
    }

    protected virtual void onCurrentGroundUpdated()
    {


    }

    #endregion

    #region PRIVATE

    /// <summary>
    /// Raycasts the shape of either the collider or the rigidbody. Will update the hit buffer array defined in this class
    /// </summary>
    /// <param name="i_direction">The direction in which we want to cast from the shape</param>
    /// <param name="i_distance">The length of the ray we want to cast</param>
    /// <param name="i_raycastSource">Whether we cast from the collider shape or the whole rigidbody</param>
    /// <returns>The number of hits</returns>
    int cast(Vector2 i_direction, float i_distance, RaycastSource i_raycastSource)
    {
        int hitCount = 0;
        if (i_raycastSource == RaycastSource.RIGIDBODY) hitCount = null == objectRgbd2D ? 0 : objectRgbd2D.Cast(i_direction, contactFilter, hitBuffer, i_distance);
        else if (i_raycastSource == RaycastSource.COLLIDER) hitCount = null == objectCollider ? 0 : objectCollider.Cast(i_direction, contactFilter, hitBuffer, i_distance);

        return hitCount;
    }

    private Vector2 updateMovement(Vector2 i_move, bool i_yMovement)
    {
        float distance = i_move.magnitude;

        List<RaycastHit2D> hitBufferList = i_yMovement ? hitBufferListY : hitBufferListX;
        groundTransformsBuffer.Clear();

        // Only check for collisions if they are enable and if the object has actually moved
        if (true == physicsConfig.IsCollisionEnabled && distance > physicsConfig.MinMoveDistance)
        {
            RaycastSource raycastSource = physicsConfig.RaycastSource;
            Vector2 currentNormal;
            RaycastHit2D hit;
            float minGroundNormalY = physicsConfig.MinGroundNormalY;

            // We cast in the move direction (either in front of us if we are checking collisions on x or under us if we are checking collisions on y)
            // We also add a shell radius to the ray length. This is to make sure we don't get stuck in another collider.
            float shellRadius = physicsConfig.ShellRadius;
            int hitCount = cast(i_move, distance + shellRadius, raycastSource);

            hitBufferList.Clear();

            for (int i = 0; i < hitCount; i++)
            {
                hit = hitBuffer[i];

                if (true == hit)
                {
                    // We check the normals of our hits in order to determine the angle of the collision 
                    currentNormal = hit.normal;
                    hitBufferList.Add(hit);

                    if (true == canCheckForCollision(ref i_move, i_yMovement, hit))
                    {
                        // Entity will be grounded only if the hit's normal has an angle
                        // on which we can actually stand
                        if (currentNormal.y > minGroundNormalY)
                        {
                            currentGroundHit = hit.point;
                            lastGroundEnterPosition = hit.point;

                            isGrounded = true;

                            if (false == groundTransformsBuffer.ContainsKey(hit.transform))
                                groundTransformsBuffer.Add(hit.transform, hit);

                            if (true == i_yMovement)
                            {
                                groundNormal = currentNormal;
                                currentNormal.x = 0f;
                            }
                        }

                        // Projecting our velocity on our current normal. This will help us scale
                        // our velocity variation depending on collision
                        float projection = Vector2.Dot(velocity, currentNormal);

                        // In that case, our projection shows us that our velocity and our normal
                        // are in opposite directions.
                        if (projection < 0f)
                        {
                            // Therefore, we cancel out the part of the velocity that has to be removed
                            // by the collision.
                            velocity -= projection * currentNormal;

                            if (false == i_yMovement)
                            {
                                if (Mathf.Abs(velocity.x) <= Physics2DConstants.EPSILON_VELOCITY) velocity.x = 0f;
                                isHittingWall = targetVelocity.x != 0f && velocity.x == 0f;
                            }
                        }

                        // The final step is to check if our given distance is less than our shell size.
                        // if it is, we fix the distance in order to avoid getting stuck inside other colliders
                        float modifiedDistance = hit.distance - shellRadius;
                        distance = modifiedDistance < distance ? modifiedDistance : distance;
                    }
                }
            }
        }

        return i_move.normalized * distance;
    }

    void triggerWallEvents()
    {
        if (null == physicsConfig) return;
        if (false == physicsConfig.IsCollisionEnabled) return;

        if (false == wasHittingWall && true == isHittingWall)
        {
            OnWallStatusChanged?.Invoke(this);
            onWallHitEnter();
        }
        else if (true == wasHittingWall && false == isHittingWall)
        {
            OnWallStatusChanged?.Invoke(this);
            onWallHitExit();
        }
    }

    void triggerGroundedEvents()
    {
        if (null == physicsConfig) return;
        if (false == physicsConfig.IsCollisionEnabled) return;

        if (false == isGrounded && true == wasGrounded)
        {
            objectTransform?.SetParent(initialParent);
            currentGroundCmpt = null;
            currentGroundTransform = null;
            OnGroundedStatusChanged?.Invoke(this);
            onExitGround();
            return;
        }

        float dist = float.MaxValue;
        Transform newTransform = currentGroundTransform;
        float castCentroidX, hitPosX, currDist;
        foreach (KeyValuePair<Transform, RaycastHit2D> pair in groundTransformsBuffer)
        {
            castCentroidX = pair.Value.centroid.x;
            hitPosX = pair.Value.point.x;
            currDist = Mathf.Abs(castCentroidX - hitPosX);

            if (currDist < dist)
            {
                dist = currDist;
                newTransform = pair.Key;
                lastGroundEnterPosition = pair.Value.point;
            }
        }

        bool didGroundTransformChange = currentGroundTransform != newTransform;
        if (null != newTransform) currentGroundLayer = newTransform.gameObject.layer;
        currentGroundTransform = newTransform;

        if (null != currentGroundTransform)
        {
            if (true == isGrounded && false == wasGrounded)
            {
                // Moving ground WIP
               /* LayerMask movingGroundLayers = physicsConfig.MovingGroundLayerMask;
                bool isMoving = movingGroundLayers == (movingGroundLayers | (1 << currentGroundLayer));

                if (true == isMoving)
                    objectTransform.SetParent(currentGroundTransform);
                else
                    objectTransform.SetParent(initialParent);*/

                OnGroundedStatusChanged?.Invoke(this);
                onEnterGround();
            }

            if (true == didGroundTransformChange)
            {
                previousGroundCmpt = currentGroundCmpt;
                currentGroundCmpt = currentGroundTransform.GetComponent<Ground2D>();
                if (currentGroundCmpt != previousGroundCmpt)
                {
                    OnDidUpdateCurrentGroundData?.Invoke(this);
                }

                OnDidUpdateCurrentGroundTransform?.Invoke(this);
                onCurrentGroundUpdated();
            }
        }

        groundTransformsBuffer.Clear();
    }

    private void initBody()
    {
        if (null == physicsConfig) return;
        if (null == objectRgbd2D) return;

        resetValues();

        objectRgbd2D.isKinematic = true;
        objectTransform = objectRgbd2D.transform;
        initialParent = objectTransform.parent;

        setContactFilters();

        int bufferSize = physicsConfig.CollisionBufferSize;
        hitBuffer = new RaycastHit2D[bufferSize];
        hitBufferListX = new List<RaycastHit2D>(bufferSize);
        hitBufferListY = new List<RaycastHit2D>(bufferSize);
        groundTransformsBuffer = new Dictionary<Transform, RaycastHit2D>(bufferSize);
    }

    /// <summary>
    /// This setting will filter the layers with which we want check collisions 
    /// </summary>
    private void setContactFilters()
    {
        contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = false;
        if(null != physicsConfig) contactFilter.SetLayerMask(physicsConfig.GroundLayerMask);
        contactFilter.useLayerMask = true;
    }

    void resetValues()
    {
        targetVelocity = MathConstants.VECTOR_2_ZERO;
        velocity = MathConstants.VECTOR_2_ZERO;
        isGrounded = false;
        wasGrounded = false;
        isHittingWall = false;
        wasHittingWall = false;
        currentGroundHit = null;
        lastGroundEnterPosition = null;
        currentGroundTransform = null;
        currentGroundLayer = null;
        groundNormal = MathConstants.VECTOR_2_ZERO;
        currentGroundCmpt = null;
        previousGroundCmpt = null;
    }

    #endregion

    #region GIZMOS

#if UNITY_EDITOR

    void drawGizmos()
    {
        if (velocity != MathConstants.VECTOR_2_ZERO)
        {
            Vector3 vel = new Vector3(velocity.x, velocity.y);
            if (true == normalizeVelocityGizmo) vel.Normalize();
            GizmoUtility.DrawArrow(transform.position, transform.position + vel, gizmoThickness, velocityColor);
        }

        if (null != currentGroundHit)
        {
            Vector3 hit = new Vector3(currentGroundHit.Value.x, currentGroundHit.Value.y);
            Vector3 normal = new Vector3(groundNormal.x, groundNormal.y).normalized;
            Vector3 groundTangent = new Vector3(groundNormal.y, -groundNormal.x).normalized;

            if (true == isGrounded)
            {
                GizmoUtility.DrawArrow(hit, hit + groundTangent, gizmoThickness, groundTangentColor);
                GizmoUtility.DrawArrow(hit, hit + normal, gizmoThickness, groundNormalColor);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (gizmoDrawMode == GizmoDrawMode.ALWAYS) drawGizmos();
    }

    void OnDrawGizmosSelected()
    {
        if (gizmoDrawMode == GizmoDrawMode.ON_SELECTED) drawGizmos();
    }

#endif

    #endregion
}
