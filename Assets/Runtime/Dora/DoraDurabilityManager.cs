using System;
using System.Collections;
using UnityEngine;

public class DoraDurabilityManager : MonoBehaviourBase
{
    [SerializeField] private DoraCellMap cellMap = null;
    [SerializeField] [Range(0.0f, 1.0f)] private float burnThreshold = 0.3f;

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

    public void InitKernels()
    {
        InitializeKernelDurability();
    }

    [ExposePublicMethod]
    public bool InitializeKernelDurability()
    {
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
                currCellData.SetDurability(rng);
                currCellData.UpdateColor();
            }
        }

        return true;
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

            Debug.LogError("burnt: " + burntKernels);
            Debug.LogError("total: " + totalKernels);

            if (totalKernels > 0)
            {
                if ((burntKernels / (float)totalKernels) > burnThreshold)
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
