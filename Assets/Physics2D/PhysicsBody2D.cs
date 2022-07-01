using System.Collections.Generic;
using UnityEngine;

public class PhysicsBody2D : MonoBehaviour
{
    [Header("Collision")]
    [SerializeField] private bool enableCollisions = true;
    [SerializeField] private RaycastSource raycastMethod = RaycastSource.RIGIDBODY;
    [SerializeField] [Range(2, 24)] private int collisionBufferSize = 16;
    [SerializeField] private float minGroundNormalY = 0.65f;
    [SerializeField] private float minMoveDistance = 0.001f;
    [SerializeField] protected float shellRadius = 0.01f;
    [SerializeField] private float epsilon = 0.0001f;

    [Header("Layer masks")]
    [SerializeField] private LayerMask collisionLayerMask = 0;

    [Header("Gravity")]
    [SerializeField] private float gravityModifier = 1f;

    private Vector2 targetVelocity = Vector2.zero;
    private Vector2 velocity = Vector2.zero;
    private ContactFilter2D contactFilter = new ContactFilter2D();
    private RaycastHit2D[] hitBuffer = null;
    private List<RaycastHit2D> hitBufferListX = null;
    private List<RaycastHit2D> hitBufferListY = null;
    private Dictionary<Transform, RaycastHit2D> groundTransformsBuffer = null;

    private bool isGrounded = false;
    private bool wasGrounded = false;
    private bool isHittingWall = false;
    private bool wasHittingWall = false;
    private Vector2? currentGroundHit = null;
    private Vector2? lastGroundEnterPosition = null;
    private Transform currentGroundTransform = null;
    private Vector2 groundNormal = Vector2.zero;

    private Collider2D objectCollider = null;
    private Rigidbody2D objectRgbd2D = null;
    private Transform objectTransform = null;


    #region UNITY AND CORE

    private void Awake()
    {
        resetValues();

        objectRgbd2D = GetComponent<Rigidbody2D>();
        objectCollider = GetComponent<Collider2D>();
        objectTransform = transform;

        hitBuffer = new RaycastHit2D[collisionBufferSize];
        hitBufferListX = new List<RaycastHit2D>(collisionBufferSize);
        hitBufferListY = new List<RaycastHit2D>(collisionBufferSize);
        groundTransformsBuffer = new Dictionary<Transform, RaycastHit2D>(collisionBufferSize);
    }

    private void FixedUpdate()
    {
        wasGrounded = isGrounded;
        isGrounded = false;

        wasHittingWall = isHittingWall;
        isHittingWall = false;

        velocity += gravityModifier * Physics2D.gravity * Time.fixedDeltaTime;
        velocity.x = targetVelocity.x;

        Vector2 deltaPosition = velocity * Time.fixedDeltaTime;
        Vector2 xMov = Vector2.zero;
        Vector2 yMov = Vector2.zero;

        xMov = updateMovement(new Vector2(groundNormal.y, -groundNormal.x) * deltaPosition.x, false);
        transform.position += new Vector3(xMov.x, xMov.y, 0f);
        yMov = updateMovement(Vector2.up * deltaPosition.y, true);
        transform.position += new Vector3(yMov.x, yMov.y, 0f);
    }

    #endregion

    #region PROTECTED VIRTUAL

    protected virtual bool canCheckForCollision(ref Vector2 i_move, bool i_yMovement, RaycastHit2D i_hit)
    {
        return true;
    }

    #endregion

    #region PRIVATE

    private Vector2 updateMovement(Vector2 i_move, bool i_yMovement)
    {
        float distance = i_move.magnitude;

        List<RaycastHit2D> hitBufferList = i_yMovement ? hitBufferListY : hitBufferListX;
        groundTransformsBuffer.Clear();

        if (true == enableCollisions && distance > minMoveDistance)
        {
            Vector2 currentNormal;
            RaycastHit2D hit;

            float collisionDistance = distance + shellRadius;
            int hitCount = 0; 
            
            if (raycastMethod == RaycastSource.RIGIDBODY) hitCount = null == objectRgbd2D ? 0 : objectRgbd2D.Cast(i_move, contactFilter, hitBuffer, collisionDistance);
            else if (raycastMethod == RaycastSource.COLLIDER) hitCount = null == objectCollider ? 0 : objectCollider.Cast(i_move, contactFilter, hitBuffer, collisionDistance);

            hitBufferList.Clear();

            for (int i = 0; i < hitCount; i++)
            {
                hit = hitBuffer[i];

                if (true == hit)
                {
                    currentNormal = hit.normal;
                    hitBufferList.Add(hit);

                    if (true == canCheckForCollision(ref i_move, i_yMovement, hit))
                    {
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

                        float projection = Vector2.Dot(velocity, currentNormal);

                        if (projection < 0f)
                        {
                            velocity -= projection * currentNormal;

                            if (false == i_yMovement)
                            {
                                if (Mathf.Abs(velocity.x) <= epsilon) velocity.x = 0f;
                                isHittingWall = targetVelocity.x != 0f && velocity.x == 0f;
                            }
                        }

                        float modifiedDistance = hit.distance - shellRadius;
                        distance = modifiedDistance < distance ? modifiedDistance : distance;
                    }
                }
            }
        }

        return i_move.normalized * distance;
    }

    void resetValues()
    {
        wasGrounded = false;
        isGrounded = false;
        wasHittingWall = false;
        isHittingWall = false;
        currentGroundTransform = null;
        currentGroundHit = null;
        lastGroundEnterPosition = null;
    }

    #endregion












}


public enum RaycastSource
{
    RIGIDBODY = 0,
    COLLIDER = 1
};



public interface IGroundedObject
{
    bool IsGrounded { get; }

    Transform CurrentGroundTransform { get; }

    int? CurrentGroundLayer { get; }

    Vector2? CurrentGroundHit { get; }
}

public interface IWallDetector
{
    bool IsHittingWall { get; }
}

public interface IMovingObject2D
{
    float VelocityX { get; }


    float VelocityY { get; }

    Vector2 Velocity2D { get; }
}