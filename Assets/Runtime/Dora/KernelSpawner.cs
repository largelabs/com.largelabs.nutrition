using PathologicalGames;
using System;
using System.Collections.Generic;
using UnityEngine;

public class KernelSpawner : MonoBehaviourBase
{
    [SerializeField] private SpawnPool kernelPool = null;

    private static readonly string DORA_KERNEL = "Dora_Kernel";

    private List<DoraKernel> livingKernels = null;

    public Action<DoraKernel> OnSpawn = null;
    public Action<DoraKernel> OnDespawn = null;
    public Action OnDespawnAll = null;

    #region UNITY API
    protected override void Awake()
    {
        base.Awake();

        livingKernels = new List<DoraKernel>();
    }
    #endregion

    #region PUBLIC API
    public IReadOnlyList<DoraKernel> LivingKernels => livingKernels;

    public void registerToSpawnPool(SpawnPool i_pool)
    {
        kernelPool = i_pool;
    }

    [ExposePublicMethod]
    public DoraKernel SpawnDoraKernel(Vector3 i_worldPosition)
    {
        if (kernelPool == null)
        {
            Debug.LogError("Dora_Kernel pool is null!");
            return null;
        }

        Transform tr = kernelPool.Spawn(DORA_KERNEL);
        tr.position = i_worldPosition;

        DoraKernel ret = tr.GetComponent<DoraKernel>();

        if (ret != null)
        {
            // register to any events if needed

            ret.InitializeValues();
            livingKernels.Add(ret);
            OnSpawn?.Invoke(ret);
        }
        else
            Debug.LogError("No DoraKernel attached to kernel prefab!");

        return ret;
    }

    [ExposePublicMethod]
    public DoraKernel SpawnDoraKernelAtAnchor(Transform i_anchor)
    {
        if (kernelPool == null)
        {
            Debug.LogError("Dora_Kernel pool is null!");
            return null;
        }

        Transform tr = kernelPool.Spawn(DORA_KERNEL);
        tr.SetParent(i_anchor);
        tr.localPosition = MathConstants.VECTOR_3_ZERO;
        tr.localRotation = MathConstants.QUATERNION_IDENTITY;
        tr.localScale = MathConstants.VECTOR_3_ONE;

        DoraKernel ret = tr.GetComponent<DoraKernel>();

        if (ret != null)
        {
            // register to any events if needed

            ret.InitializeValues();
            livingKernels.Add(ret);
            OnSpawn?.Invoke(ret);
        }
        else
            Debug.LogError("No DoraKernel attached to kernel prefab!");

        return ret;
    }

    public List<DoraKernel> SpawnKernelGroup(List<Vector3> i_worldPositions)
    {
        if (i_worldPositions == null || i_worldPositions.Count < 1)
        {
            Debug.LogError("The provided world position list is invalid!");
            return null;
        }

        int length = i_worldPositions.Count;
        List<DoraKernel> ret = new List<DoraKernel>();

        for (int i = 0; i < length; i++)
        {
            ret.Add(SpawnDoraKernel(i_worldPositions[i]));
        }

        return ret;
    }


    public List<DoraKernel> SpawnKernelGroupAtAnchor(List<Transform> i_anchors)
    {
        if (i_anchors == null || i_anchors.Count < 1)
        {
            Debug.LogError("The provided anchor list is invalid!");
            return null;
        }

        int length = i_anchors.Count;
        List<DoraKernel> ret = new List<DoraKernel>();

        for (int i = 0; i < length; i++)
        {
            ret.Add(SpawnDoraKernelAtAnchor(i_anchors[i]));
        }

        return ret;
    }

    public void DespawnKernel (DoraKernel i_kernel)
    {
        despawnDoraCob(i_kernel);
        livingKernels.Remove(i_kernel);
    }

    [ExposePublicMethod]
    public void DespawnAllKernels()
    {
        if (kernelPool == null) return;
        if (livingKernels == null || livingKernels.Count < 1) return;

        int length = livingKernels.Count;
        for (int i = 0; i < length; i++)
        {
            despawnDoraCob(livingKernels[i]);
        }

        livingKernels.Clear();

        OnDespawnAll?.Invoke();
    }
    #endregion

    #region PRIVATE API
    private void despawnDoraCob(DoraKernel i_kernel)
    {
        kernelPool?.Despawn(i_kernel.transform);
        OnDespawn?.Invoke(i_kernel);

        // unregister from any events if needed
    }

    #endregion
}
