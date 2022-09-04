using System;
using System.Collections;
using UnityEngine;

public class DoraDurabilityManager : MonoBehaviourBase
{
    public enum Distribution
    {
        Normal,
        Uniform,
        ChiSquare
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
        int superKernelsSpawned = 0;
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

        DoraCellData currCellData = null;

        for (int i = 0; i < length0; i++)
        {
            for (int j = 0; j < length1; j++)
            {
                currCellData = cellMap.GetCell(new Vector2Int(i, j), false, false);
                rng = UnityEngine.Random.Range(minDurability, maxDurability);

                currCellData.SetBurnable(KernelIsBurnable());

                currCellData.SetDurability(rng);
                currCellData.UpdateColor();
            }
        }

        if (i_canSpawnSuper)
        {
            int length = batchData.MaxSuperKernelsPerCob;
            for (int i = 0; i < length; i++)
            {
                currCellData = cellMap.GetCell(getRandomCell(length0, length1), false, false);

                currCellData.SetSuper(true);
                superKernelsSpawned++;
            }
        }

        burntPercentage = 0.0f;
        if (superKernelsSpawned > 0)
            o_superKernelSpawned = true;

        return true;
    }

    private Vector2Int getRandomCell(int i_length0, int i_length1)
    {
        Vector2Int ret = new Vector2Int(0, 0);

        ret.x = UnityEngine.Random.Range(0, i_length0);
        ret.y = UnityEngine.Random.Range(0, i_length1);

        return ret;
    }

    private bool KernelIsBurnable()
    {
        throw new NotImplementedException();
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
