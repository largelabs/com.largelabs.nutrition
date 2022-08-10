
using UnityEngine;

public interface IGravity2DConfig
{
    /// <summary>
    /// The objects gravity modifier
    /// </summary>
    public float GravityModifier { get; }

    public Vector2 GravityVector { get; }
}

