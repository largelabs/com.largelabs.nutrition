using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIKernelManager : MonoBehaviourBase
{
    [SerializeField] DoraScoreManager scoreManager = null;
    [SerializeField] UIKernelSpawner uiKernelSpawner = null;

    [Header("Sequencing")]
    [SerializeField] private InterpolatorsManager interpolatorsManager = null;
    [SerializeField] private AnimationCurve animCurve = null;
    [SerializeField] private RectTransform anchorStart = null;
    [SerializeField] private RectTransform anchorEnd = null;
    [SerializeField] private float timePerUIKernel = 0.2f;
    [SerializeField] private float xOffsetPerUIKernel = -60.0f;

    Coroutine kernelStackRoutine = null;

    public float TimePerUIKernel => timePerUIKernel;

    [ExposePublicMethod]
    public void SpawnAtStartAnchor()
    {
        uiKernelSpawner.SpawnUIKernelAtAnchor(anchorStart, 0f, false);
    }

    [ExposePublicMethod]
    public void SpawnMultipleAtAnchor(int i_spawnCount, float i_xOffset)
    {
        uiKernelSpawner.SpawnUIKernelsStartingFromAnchor(anchorStart, i_spawnCount, i_xOffset, false);
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
        List<RectTransform> uiKernels = new List<RectTransform>();
        ScoreKernelInfo currKernelInfo = null;

        int length = i_eatenKernels.Count;
        for (int i = 0; i < length; i++)
        {
            // add each uikernel to a list and use it to play the animations in the next loop
            uiKernels.Add(uiKernelSpawner.SpawnUIKernelAtAnchor(anchorStart, xOffsetPerUIKernel * i, 
                            i_eatenKernels[i].KernelStatus == DoraKernel.KernelStatus.Burnt));
        }

        for (int i = 0; i < length; i++)
        {
            currKernelInfo = i_eatenKernels[i];
            //some animation stuff
            yield return StartCoroutine(uiKernelMovementSequence(uiKernels[i], timePerUIKernel / 2));
            scoreManager.AddScoreByStatus(currKernelInfo.KernelStatus,
                                          currKernelInfo.ScoreMultiplier,
                                          anchorStart.position,
                                          1.0f, 0.01f, 20f);
            yield return StartCoroutine(scoreUpdateSequence(timePerUIKernel / 2));
        }

        Debug.LogError("done with kernel stack");

        this.DisposeCoroutine(ref kernelStackRoutine);
    }

    private IEnumerator uiKernelMovementSequence(RectTransform i_uiKernel, float i_timeAvailable)
    {
        UIElementMove elementMove = i_uiKernel.GetComponent<UIElementMove>();
        if (elementMove != null)
            elementMove.moveToRectTransform(anchorEnd, i_timeAvailable, interpolatorsManager, animCurve, null);
        yield return this.Wait(i_timeAvailable/2f);

        UIElementAlpha elementAlpha = i_uiKernel.GetComponent<UIElementAlpha>();
        if (elementAlpha != null)
            elementAlpha.lerpAlpha(1f, 0f, i_timeAvailable, interpolatorsManager, animCurve, null);
        yield return this.Wait(i_timeAvailable / 2f);

        uiKernelSpawner.DespawnKernel(i_uiKernel);
    } 
    
    private IEnumerator scoreUpdateSequence(float i_timeAvailable)
    {
        // call scorepopup with random pos anchored below score
        yield return this.Wait(i_timeAvailable);
    }
}
