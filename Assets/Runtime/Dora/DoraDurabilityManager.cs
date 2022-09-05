using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoraDurabilityManager : MonoBehaviourBase
{
    public enum Distribution
    {
        CenterFocused,
        EdgeFocused,
        Sparse,
        Uniform
    }

    [SerializeField] private DoraCellMap cellMap = null;
    [SerializeField] [Range(0.0f, 1.0f)] private float burnThreshold = 0.3f;

    private DoraBatchData batchData = null;

    private float burntPercentage = 0.0f;
    public Action OnPassBurnThreshold = null;

    List<Vector2Int> directions = new List<Vector2Int>
            {   Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right,
                Vector2Int.up,
                Vector2Int.up + Vector2Int.right,
                Vector2Int.up + Vector2Int.left,
                Vector2Int.down + Vector2Int.right,
                Vector2Int.down + Vector2Int.left
            };

    Coroutine updateDurabilityRoutine = null;

    #region UNITY AND CORE

    protected override void Awake()
    {
        base.Awake();
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
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

                if (KernelIsBurnable(i, j, length1 - 1, totalBurnable))
                {
                    currCellData.SetBurnable(true);
                    totalBurnable++;
                }
                else
                    currCellData.SetBurnable(false);

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
            currCellData.SetBurnable(false);
            currCellData.SetDurability(1f);
            currCellData.UpdateColor();

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

    private bool KernelIsBurnable(int i_rowIdx, int i_columnIdx, int i_maxColumnIdx, int i_totalBurnable)
    {
        float maxBurn = batchData.MaxBurntPercentage;
        if (i_totalBurnable >= maxBurn * cellMap.TotalCellCount)
            return false;

        Distribution distro = batchData.DistributionStyle;

        int halfwayIdx = i_maxColumnIdx / 2;
        int maxDiff = halfwayIdx;
        int minDiff = 0;
        int diff = Mathf.Abs(halfwayIdx - i_columnIdx);
        float indexDistribution = (float)(diff - minDiff) / (maxDiff - minDiff);
        float calculatedChance = 0f;
        float multipliedChance = 0.65f;

        if (distro == Distribution.Uniform)
        {
            // completely random
            calculatedChance = maxBurn;
        }
        else if (distro == Distribution.CenterFocused)
        {
            // lerp chance to be max at center and lower towards the edges
            calculatedChance = (1 - indexDistribution) * multipliedChance;
        }
        else if (distro == Distribution.EdgeFocused)
        {
            // lerp chance to be max at edges and lower towards the center
            calculatedChance = indexDistribution * multipliedChance;

        }
        else if (distro == Distribution.Sparse)
        {
            // max chance by default and decrease chance for each adjacent burnable kernel;
            calculatedChance = multipliedChance;
            float chanceReduction = calculatedChance / 2;
            Vector2Int baseCoord = new Vector2Int(i_rowIdx, i_columnIdx);
            foreach (Vector2Int direction in directions)
            {
                bool? kernelBurnable = cellMap.GetCell(baseCoord + direction, false, false).KernelIsBurnable();
                if(kernelBurnable != null && kernelBurnable.Value == true)
                    calculatedChance -= chanceReduction;
            }
        }
        else
        {
            Debug.LogError("Invalid distribution style!");
            return false;
        }

        return (UnityEngine.Random.Range(0f, 1f) <= calculatedChance);
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
                        if (durability.Value == 0f && currCellData.KernelIsBurnable().Value == true)
                            burntKernels++;
                    }

                    if (currCellData.KernelIsSuper().Value == false)
                    {
                        currCellData.DecreaseDurability(durabilityLossPerInterval);
                        currCellData.UpdateColor();
                    }
                }
            }

           // Debug.LogError("burnt: " + burntKernels);
           // Debug.LogError("total: " + totalKernels);

            if (totalKernels > 0)
            {
                burntPercentage = (burntKernels / (float)totalKernels);
                Debug.LogError("Burn percentage: " + burntPercentage);
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
