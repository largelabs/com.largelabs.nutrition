using UnityEngine;

/// <summary>
/// Base MonoBehaviour that automatically caches the transform.
/// Inherit from this class and access transform as usual.
/// MonoBehaviourBase also enables some useful editor features.
/// </summary>
public class MonoBehaviourBase : MonoBehaviour
{
    private Transform cachedTransform = null;

    #region UNITY AND CORE

    protected virtual void Awake()
    {
        cacheTransform();
    }

    [HideInInspector]
    new public Transform transform
    {
        get
        {
            if (null == cachedTransform)
                cacheTransform();
            return cachedTransform;
        }
    }

    #endregion

    #region PROTECTED

    protected void cacheTransform()
    {
        cachedTransform = base.transform;
    }

    #endregion
}
