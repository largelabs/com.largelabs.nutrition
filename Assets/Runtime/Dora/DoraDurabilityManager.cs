using System;
using System.Collections;
using UnityEngine;

public class DoraDurabilityManager : MonoBehaviourBase
{
    public enum Distribution
    {
        DenseMiddle,
        DenseEdges,
        Sparse,
        Uniform
    }

    [SerializeField] private DoraCellMap cellMap = null;
    [SerializeField] [Range(0.0f, 1.0f)] private float burnThreshold = 0.3f;

    private DoraBatchData batchData = null;

    private float burntPercentage = 0.0f;
    public Action OnPassBurnThreshold = null;

    Coroutine updateDurabilityRoutine = null;

    #region UNITY AND CORE

    protected override void Awake()
    {
        base.Awake();
        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
    }

    #endregion

    #region PUBLIC API
    public float BurntPercentage => burntPercentage;
    public float BurnThreshold => burnThreshold;
    public bool IsPastBurnThreshold => (burntPercentage > burnThreshold);

    public void SetBatchData(DoraBatchData i_doraBatchData)
    {
        batchData = i_doraBatchData;
    }

    [ExposePublicMethod]
    public bool InitializeKernelDurability(bool i_canSpawnSuper, out bool o_superKernelSpawned)
    {
        o_superKernelSpawned = false;

        if (batchData == null)
        {
            Debug.LogError("No batch data provided!");
            return false;
        }

        if (cellMap == null)
        {
            Debug.LogError("cell map unavailable");
            return false;
        }

        int length0 = cellMap.CellMapLength0;
        int length1 = cellMap.CellMapLength1;

        float maxDurability = cellMap.DoraData.MaxInitialDurability;
        float minDurability = cellMap.DoraData.MinInitialDurability;

        float rng = 0f;

        int totalBurnable = 0;

        DoraCellData currCellData = null;

        for (int i = 0; i < length0; i++)
        {
            for (int j = 0; j < length1; j++)
            {
                currCellData = cellMap.GetCell(new Vector2Int(i, j), false, false);
                rng = UnityEngine.Random.Range(minDurability, maxDurability);

                if (currCellData.SetBurnable(KernelIsBurnable(j, length1 - 1, totalBurnable)))
                    totalBurnable++;

                currCellData.SetDurability(rng);
                currCellData.UpdateColor();
            }
        }

        if (i_canSpawnSuper)
            o_superKernelSpawned = setSuperKernels(length0, length1);

        burntPercentage = 0.0f;

        return true;
    }

    private bool setSuperKernels(int i_length0, int i_length1)
    {
        DoraCellData currCellData = null;

        int superKernelsSpawned = 0;
        int length = batchData.MaxSuperKernelsPerCob;

        for (int i = 0; i < length; i++)
        {
            currCellData = cellMap.GetCell(getRandomCellIdx(i_length0, i_length1), false, false);

            currCellData.SetSuper(true);
            superKernelsSpawned++;
        }

        return (superKernelsSpawned > 0);
    }

    private Vector2Int getRandomCellIdx(int i_length0, int i_length1)
    {
        Vector2Int ret = new Vector2Int(0, 0);

        ret.x = UnityEngine.Random.Range(0, i_length0);
        ret.y = UnityEngine.Random.Range(0, i_length1);

        return ret;
    }

    private bool KernelIsBurnable(int i_columnIdx, int i_maxColumnIdx, int i_totalBurnable)
    {
        if (i_totalBurnable >= batchData.MaxBurntPercentage * cellMap.TotalCellCount)
            return false;

        Distribution distro = batchData.DistributionStyle;

        if (distro == Distribution.Uniform)
        {
            // completely random
            // use max chance
        }
        else if (distro == Distribution.DenseMiddle)
        {
            // lerp chance to be max at center and lower towards the edges
        }
        else if (distro == Distribution.DenseEdges)
        {
            // lerp chance to be max at edges and lower towards the center
        }
        else if (distro == Distribution.Sparse)
        {
            // max chance by default and decrease chance for each adjacent burnable kernel;
        }
        else
        {
            Debug.LogError("Invalid distribution style!");
            return false;
        }
    }

    [ExposePublicMethod]
    public void ActivateDurabilityUpdate()
    {
        if (updateDurabilityRoutine == null)
            updateDurabilityRoutine = StartCoroutine(updateDurability());
    }

    [ExposePublicMethod]
    public void DeactivateDurabilityUpdate()
    {
        this.DisposeCoroutine(ref updateDurabilityRoutine);
        updateDurabilityRoutine = null;
    }

    #endregion

    #region PRIVATE

    private IEnumerator updateDurability()
    {
        while (cellMap == null)
        {
            Debug.LogError("cell map unavailable");
            yield return null;
        }

        int length0 = cellMap.CellMapLength0;
        int length1 = cellMap.CellMapLength1;

        if (cellMap.DoraData == null)
        {
            Debug.LogError("No Dora Data assigned! Breaking...");
            DeactivateDurabilityUpdate();
            yield break;
        }

        float durabilityLossInterval = cellMap.DoraData.DurabilityLossInterval;
        float durabilityLossPerInterval = cellMap.DoraData.DurabilityLossPerInterval;

        float? durability = 0f;
        DoraCellData currCellData = null;

        while (true)
        {
            int totalKernels = 0;
            int burntKernels = 0;

            for (int i = 0; i < length0; i++)
            {
                for (int j = 0; j < length1; j++)
                {
                    currCellData = cellMap.GetCell(new Vector2Int(i, j), false, false);
                    durability = currCellData.GetDurability();
                    if (durability != null)
                    {
                        totalKernels++;
                        if (durability.Value == 0f)
                            burntKernels++;
                    }

                    currCellData.DecreaseDurability(durabilityLossPerInterval);
                    currCellData.UpdateColor();
                }
            }

           // Debug.LogError("burnt: " + burntKernels);
           // Debug.LogError("total: " + totalKernels);

            if (totalKernels > 0)
            {
                burntPercentage = (burntKernels / (float)totalKernels);
                if (IsPastBurnThreshold)
                {
                    OnPassBurnThreshold?.Invoke();
                    Debug.LogError("Burn Threshold: " + burnThreshold + " passed!");
                }
            }

            yield return this.Wait(durabilityLossInterval);
        }
    }

    #endregion
}
