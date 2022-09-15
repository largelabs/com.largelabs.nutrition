using PathologicalGames;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DoraSpawner : MonoBehaviourBase
{
    [SerializeField] private SpawnPool doraCobPool = null;
    [SerializeField] private SpawnPool doraKernelPool = null;

    private static readonly string DORA_COB = "Dora_Cob";

    private List<DoraCellMap> livingDora = null;

    public Action<DoraCellMap> OnSpawn = null;
    public Action<DoraCellMap> OnDespawn = null;
    public Action OnDespawnAll = null;

    #region UNITY API
    protected override void Awake()
    {
        base.Awake();

        livingDora = new List<DoraCellMap>();
    }
    #endregion

    #region PUBLIC API
    public IReadOnlyList<DoraCellMap> LivingDora => livingDora;

    [ExposePublicMethod]
    public DoraCellMap SpawnDoraCob(Vector3 i_worldPosition)
    {
        if (doraCobPool == null)
        {
            Debug.LogError("Dora_Cob pool is null!");
            return null;
        }

        Transform tr = doraCobPool.Spawn(DORA_COB);
        tr.position = i_worldPosition;

        DoraCellMap ret = tr.GetComponent<DoraCellMap>();

        if (ret != null)
        {
            KernelSpawner kernelSpawner = tr.GetComponentInChildren<KernelSpawner>();
            kernelSpawner.registerToSpawnPool(doraKernelPool);

            // register to any events if needed

            livingDora.Add(ret);
            OnSpawn?.Invoke(ret);
        }
        else
            Debug.LogError("No DoraCellMap attached to cob prefab!");

        return ret;
    }

    public DoraCellMap SpawnDoraCobAtAnchor(Transform i_anchor)
    {
        if (doraCobPool == null)
        {
            Debug.LogError("Dora_Cob pool is null!");
            return null;
        }

        Transform tr = doraCobPool.Spawn(DORA_COB);
        tr.SetParent(i_anchor);
        tr.localPosition = MathConstants.VECTOR_3_ZERO;
        tr.localRotation = MathConstants.QUATERNION_IDENTITY;
        tr.localScale = MathConstants.VECTOR_3_ONE;

        DoraCellMap ret = tr.GetComponent<DoraCellMap>();

        if (ret != null)
        {
            KernelSpawner kernelSpawner = tr.GetComponentInChildren<KernelSpawner>();
            kernelSpawner.registerToSpawnPool(doraKernelPool);

            // register to any events if needed

            livingDora.Add(ret);
            OnSpawn?.Invoke(ret);
        }
        else
            Debug.LogError("No DoraCellMap attached to cob prefab!");

        return ret;
    }

    public List<DoraCellMap> SpawnDoraCobGroup(List<Vector3> i_worldPositions)
    {
        if (i_worldPositions == null || i_worldPositions.Count < 1)
        {
            Debug.LogError("The provided world position list is invalid!");
            return null;
        }

        int length = i_worldPositions.Count;
        List<DoraCellMap> ret = new List<DoraCellMap>();

        for (int i = 0; i < length; i++)
        {
            ret.Add(SpawnDoraCob(i_worldPositions[i]));
        }

        return ret;
    }

    public List<DoraCellMap> SpawnDoraCobGroupAtAnchor(List<Transform> i_anchors)
    {
        if (i_anchors == null || i_anchors.Count < 1)
        {
            Debug.LogError("The provided anchor list is invalid!");
            return null;
        }

        int length = i_anchors.Count;
        List<DoraCellMap> ret = new List<DoraCellMap>();

        for (int i = 0; i < length; i++)
        {
            ret.Add(SpawnDoraCobAtAnchor(i_anchors[i]));
        }

        return ret;
    }

    public void DespawnDoraCob(DoraCellMap i_doraCob)
    {
        Debug.LogError("DESPAWNED: " + i_doraCob.gameObject);
        despawnDoraCob(i_doraCob);
        livingDora.Remove(i_doraCob);
    }

    [ExposePublicMethod]
    public void DespawnAllDora()
    {
        if (doraCobPool == null) return;
        if (livingDora == null || livingDora.Count < 1) return;

        int length = livingDora.Count;
        for (int i = 0; i < length; i++)
        {
            despawnDoraCob(livingDora[i]);
        }

        livingDora.Clear();

        OnDespawnAll?.Invoke();
    }
    #endregion

    #region PRIVATE API
    private void despawnDoraCob(DoraCellMap i_doraCob)
    {
        i_doraCob.ReleaseDoraCob();
        doraCobPool?.Despawn(i_doraCob.transform);
        OnDespawn?.Invoke(i_doraCob);

        // unregister from any events if needed
    }

    #endregion
}
