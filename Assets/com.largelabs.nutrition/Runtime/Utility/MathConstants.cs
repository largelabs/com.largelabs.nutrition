using UnityEngine;

/// <summary>
/// Using these constants to initialize Vector variables will improve performance.
/// Use these instead of Vector3.zero etc... 
/// </summary>
public static class MathConstants
{
    public static readonly Vector3 VECTOR_3_ZERO = Vector3.zero;
    public static readonly Vector3 VECTOR_3_ONE = Vector3.one;
    public static readonly Vector3 VECTOR_3_UP = Vector3.up;
    public static readonly Vector3 VECTOR_3_DOWN = Vector3.down;
    public static readonly Vector3 VECTOR_3_LEFT = Vector3.left;
    public static readonly Vector3 VECTOR_3_RIGHT = Vector3.right;
    public static readonly Vector3 VECTOR_3_FORWARD = Vector3.forward;
    public static readonly Vector3 VECTOR_3_BACK = Vector3.back;

    public static readonly Vector3Int VECTOR_3_INT_ZERO = Vector3Int.zero;
    public static readonly Vector3Int VECTOR_3_INT_ONE = Vector3Int.one;
    public static readonly Vector3Int VECTOR_3_INT_UP = Vector3Int.up;
    public static readonly Vector3Int VECTOR_3_INT_DOWN = Vector3Int.down;
    public static readonly Vector3Int VECTOR_3_INT_LEFT = Vector3Int.left;
    public static readonly Vector3Int VECTOR_3_INT_RIGHT = Vector3Int.right;
    public static readonly Vector3Int VECTOR_3_INT_FORWARD = Vector3Int.forward;
    public static readonly Vector3Int VECTOR_3_INT_BACK = Vector3Int.back;

    public static readonly Vector2 VECTOR_2_ZERO = Vector2.zero;
    public static readonly Vector2 VECTOR_2_ONE = Vector2.one;
    public static readonly Vector2 VECTOR_2_UP = Vector2.up;
    public static readonly Vector2 VECTOR_2_DOWN = Vector2.down;
    public static readonly Vector2 VECTOR_2_LEFT = Vector2.left;
    public static readonly Vector2 VECTOR_2_RIGHT = Vector2.right;

    public static readonly Vector2Int VECTOR_2_INT_ZERO = Vector2Int.zero;
    public static readonly Vector2Int VECTOR_2_INT_ONE = Vector2Int.one;
    public static readonly Vector2Int VECTOR_2_INT_UP = Vector2Int.up;
    public static readonly Vector2Int VECTOR_2_INT_DOWN = Vector2Int.down;
    public static readonly Vector2Int VECTOR_2_INT_LEFT = Vector2Int.left;
    public static readonly Vector2Int VECTOR_2_INT_RIGHT = Vector2Int.right;
}

