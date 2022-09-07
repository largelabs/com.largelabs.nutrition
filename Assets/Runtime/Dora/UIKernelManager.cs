using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIKernelManager : MonoBehaviourBase
{
    [SerializeField] DoraScoreManager scoreManager = null;
    [SerializeField] UIKernelSpawner uiKernelSpawner = null;
    [SerializeField] RectTransform anchorStart = null;
    [SerializeField] RectTransform anchorEnd = null;
    [SerializeField] private float timePerUIKernel = 0.2f;
    [SerializeField] private float xOffsetPerUIKernel = -60.0f;

    Coroutine kernelStackRoutine = null;

    public float TimePerUIKernel => timePerUIKernel;

    [ExposePublicMethod]
    public void SpawnAtStartAnchor()
    {
        uiKernelSpawner.SpawnUIKernelAtAnchor(anchorStart, 0f);
    }

    [ExposePublicMethod]
    public void SpawnMultipleAtAnchor(int i_spawnCount, float i_xOffset)
    {
        uiKernelSpawner.SpawnUIKernelsStartingFromAnchor(anchorStart, i_spawnCount, i_xOffset);
    }

    public void HandleKernelStack(List<ScoreKernelInfo> i_eatenKernels)
    {
        if (i_eatenKernels == null || i_eatenKernels.Count == 0)
        {
            Debug.LogError("Invalid eaten kernel collection! Returning...");
            return;
        }

        if (kernelStackRoutine == null)
            kernelStackRoutine = StartCoroutine(kernelStackSequence(i_eatenKernels));
    }

    private IEnumerator kernelStackSequence(List<ScoreKernelInfo> i_eatenKernels)
    {
        ScoreKernelInfo currKernelInfo = null;
        int length = i_eatenKernels.Count;
        for (int i = 0; i < length; i++)
        {
            uiKernelSpawner.SpawnUIKernelAtAnchor(anchorStart, xOffsetPerUIKernel * i);
        }

        for (int i = 0; i < length; i++)
        {
            currKernelInfo = i_eatenKernels[i];
            //some animation stuff
            yield return this.Wait(timePerUIKernel / 2);
            scoreManager.AddScoreByStatus(currKernelInfo.KernelStatus,
                                          currKernelInfo.ScoreMultiplier,
                                          anchorStart.position,
                                          1.0f, 0.01f, 20f);
            yield return this.Wait(timePerUIKernel / 2);
        }

        this.DisposeCoroutine(ref kernelStackRoutine);
    }
}
