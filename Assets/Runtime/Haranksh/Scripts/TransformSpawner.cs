using PathologicalGames;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpawnPool))]
public abstract class TransformSpawner<ComponentType, PrefabIdType> : MonoBehaviourBase
    where ComponentType : MonoBehaviour
{
    [SerializeField] private SpawnPool transformPool = null;

    private List<ComponentType> livingTransforms = null;

    public Action<ComponentType> OnSpawn = null;
    public Action<ComponentType> OnDespawn = null;
    public Action OnDespawnAll = null;

    #region UNITY
    protected override void Awake()
    {
        base.Awake();
        livingTransforms = new List<ComponentType>();
    }
    #endregion

    #region PUBLIC API
    public IReadOnlyList<ComponentType> LivingTransforms => livingTransforms;

    [ExposePublicMethod]
    public ComponentType SpawnTransformAtAnchor(Transform i_anchor, Vector3 i_offset, PrefabIdType i_prefabId)
    {
        if (transformPool == null)
        {
            Debug.LogError("Transform Pool is null! Retuning...");
            return default;
        }

        Transform tr = transformPool.Spawn(getPrefab(i_prefabId));
        Transform originalParent = tr.parent;

        tr.SetParent(i_anchor);
        tr.localPosition = i_offset;
        tr.localRotation = MathConstants.QUATERNION_IDENTITY;
        tr.localScale = MathConstants.VECTOR_3_ONE;

        tr.SetParent(originalParent);

        ComponentType ret = tr.GetComponent<ComponentType>();

        if (ret != null)
        {
            livingTransforms.Add(ret);
            OnSpawn(ret);
        }
        else
            Debug.LogError("No" + typeof(ComponentType) + "attached to given prefab!");

        return ret;
    }

    [ExposePublicMethod]
    public ComponentType SpawnTransformAtWorldPosition(Vector3 i_worldPos, PrefabIdType i_prefabId)
    {
        if (transformPool == null)
        {
            Debug.LogError("Transform Pool is null! Retuning...");
            return default;
        }

        Transform tr = transformPool.Spawn(getPrefab(i_prefabId));
        tr.position = i_worldPos;

        ComponentType ret = tr.GetComponent<ComponentType>();

        if (ret != null)
        {
            livingTransforms.Add(ret);
            OnSpawn(ret);
        }
        else
            Debug.LogError("No" + typeof(ComponentType) + "attached to given prefab!");

        return ret;
    }

    public void DespawnTransform(ComponentType i_component)
    {
        despawnTransform(i_component);
        livingTransforms.Remove(i_component);
    }

    [ExposePublicMethod]
    public void DespawnAllTransforms()
    {
        if (transformPool == null) return;
        if (livingTransforms == null || livingTransforms.Count < 1) return;

        int length = livingTransforms.Count;
        for (int i = 0; i < length; i++)
        {
            despawnTransform(livingTransforms[i]);
        }

        livingTransforms.Clear();
    }

    protected abstract string getPrefab(PrefabIdType i_prefabId);

    protected abstract void resetComponent(ComponentType i_component);
    #endregion

    #region PRIVATE API
    private void despawnTransform(ComponentType i_component)
    {
        resetComponent(i_component);
        transformPool?.Despawn(i_component.transform);
    }
    #endregion
}
