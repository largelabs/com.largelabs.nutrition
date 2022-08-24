using System;
using System.Collections;
using UnityEngine;

public class DoraDurabilityManager : MonoBehaviour
{
    [SerializeField] private DoraCellMap cellMap = null;
    [SerializeField] [Range(0.0f, 1.0f)] private float burnThreshold = 0.3f;

    public Action OnPassBurnThreshold = null;

    Coroutine updateDurabilityRoutine = null;

    private void Awake()
    {
        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
    }

    public void InitKernels()
    {
        InitializeKernelDurability();
    }

    public bool InitializeKernelDurability()
    {
        if (cellMap == null)
        {
            Debug.LogError("cell map unavailable");
            return false;
        }

        int length0 = cellMap.cellMapLength0;
        int length1 = cellMap.cellMapLength1;

        float maxDurability = cellMap.DoraData.MaxInitialDurability;
        float minDurability = cellMap.DoraData.MinInitialDurability;

        float rng = 0f;

        for (int i = 0; i < length0; i++)
        {
            for (int j = 0; j < length1; j++)
            {
                rng = UnityEngine.Random.Range(minDurability, maxDurability);
                cellMap.SetDurability(i, j, rng);
                cellMap.UpdateColor(i, j);
            }
        }

        return true;
    }

    public void ActivateDurabilityUpdate()
    {
        if (updateDurabilityRoutine == null)
            updateDurabilityRoutine = StartCoroutine(updateDurability());
    }

    public void DeactivateDurabilityUpdate()
    {
        StopCoroutine(updateDurability());
        updateDurabilityRoutine = null;
    }

    private IEnumerator updateDurability()
    {
        while (cellMap == null)
        {
            Debug.LogError("cell map unavailable");
            yield return null;
        }

        int length0 = cellMap.cellMapLength0;
        int length1 = cellMap.cellMapLength1;

        if (cellMap.DoraData == null)
        {
            Debug.LogError("No Dora Data assigned! Breaking...");
            DeactivateDurabilityUpdate();
            yield break;
        }

        float durabilityLossInterval = cellMap.DoraData.DurabilityLossInterval;
        float durabilityLossPerInterval = cellMap.DoraData.DurabilityLossPerInterval;

        while (true)
        {
            int totalKernels = 0;
            int burntKernels = 0;
            float? durability = 0f;

            for (int i = 0; i < length0; i++)
            {
                for (int j = 0; j < length1; j++)
                {
                    durability = cellMap.GetDurability(i, j);
                    if (durability != null)
                    {
                        totalKernels++;
                        if (durability == 0f)
                            burntKernels++;
                    }

                    cellMap.DecreaseDurability(i, j, durabilityLossPerInterval);
                    cellMap.UpdateColor(i, j);
                }
            }

            if (totalKernels > 0)
            {
                if ((burntKernels / totalKernels) > burnThreshold)
                    OnPassBurnThreshold?.Invoke();
            }

            yield return this.Wait(durabilityLossInterval);
        }
    }
}
