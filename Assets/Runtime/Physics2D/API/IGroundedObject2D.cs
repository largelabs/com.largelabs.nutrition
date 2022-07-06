using UnityEngine;

public interface IGroundedObject2D
{
    bool IsGrounded { get; }

    Transform CurrentGroundTransform { get; }

    int? CurrentGroundLayer { get; }

    Vector2? CurrentGroundHit { get; }

    Vector2? LastGroundEnterPosition { get; }

    Physics2DGroundEvent OnGroundedStatusChanged { get; set; }

    Physics2DGroundEvent OnDidUpdateCurrentGroundTransform { get; set; }

    Physics2DGroundEvent OnDidUpdateCurrentGroundData { get; set; }
}

