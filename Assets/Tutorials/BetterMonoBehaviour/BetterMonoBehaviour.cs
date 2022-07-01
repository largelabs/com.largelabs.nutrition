using UnityEngine;

public class BetterMonoBehaviour : MonoBehaviourBase
{
    #region UNITY AND CORE

    // If you want to implement awake...
    protected override void Awake()
    {
        // Always call base.Awake so that transform caching can happen
        base.Awake();

        // Use the cached transform normally
        ResetPosition();
    }

    #endregion


    #region PUBLIC API

    // Use this attribute to get a button in the editor at runtime. Very useful for testing ! 
    // Works with value type parameters (int, float, bool, Vector2, Vector3...)
    [ExposePublicMethod]
    public void SetPosition(Vector3 i_pos)
    {
        transform.position = i_pos;
    }

    [ExposePublicMethod]
    public void ResetPosition()
    {
        transform.position = MathConstants.VECTOR_3_ZERO;
    }

    #endregion
}
