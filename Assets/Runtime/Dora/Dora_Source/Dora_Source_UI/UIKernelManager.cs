using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIKernelManager : MonoBehaviourBase
{
    [SerializeField] DoraScoreManager scoreManager = null;
    [SerializeField] UIKernelSpawner uiKernelSpawner = null;

    [Header("Sequencing")]
    [SerializeField] private InterpolatorsManager interpolatorsManager = null;
    [SerializeField] private AnimationCurve alphaCurve = null;
    [SerializeField] private AnimationCurve positionCurve = null;
    [SerializeField] private AnimationCurve stackShiftCurve = null;
    [SerializeField] private RectTransform anchorStart = null;
    [SerializeField] private RectTransform anchorEnd = null;
    [SerializeField] private RectTransform anchorScore = null;

    [SerializeField] private float timePerUIKernel = 0.2f;
    [SerializeField] private float timePerUIKernelFrenzy = 0.2f;
    [SerializeField] private float xOffsetPerUIKernel = -60.0f;

    [SerializeField] DoraSFXProvider sfxProvider = null;

    RectTransform lastAnchor = null;
    Vector3 anchorStartInitialAnchoredPosition = MathConstants.VECTOR_3_ZERO;
    Vector3 anchorStartInitialPosition = MathConstants.VECTOR_3_ZERO;

    Queue<UIDoraKernel> uiKernelQueue = null;
    Coroutine dequeueKernelsRoutine = null;

    #region UNITY AND CORE

    protected override void Awake()
    {
        base.Awake();
        anchorStartInitialAnchoredPosition = anchorStart.anchoredPosition;
        anchorStartInitialPosition = anchorStart.position;
    }

    #endregion

    #region PUBLIC API

    public void EnqueueKernels(Queue<ScoreKernelInfo> i_kernels)
    {
        if (null == lastAnchor)
        {
            lastAnchor = anchorStart;
        }

        if (null == uiKernelQueue) uiKernelQueue = new Queue<UIDoraKernel>();

        while (i_kernels.Count != 0)
        {
            ScoreKernelInfo kernelInfo = i_kernels.Dequeue();
            UIDoraKernel uiKernel = uiKernelSpawner.SpawnUIKernelAtAnchor(lastAnchor, xOffsetPerUIKernel, kernelInfo.KernelStatus == KernelStatus.Burnt);
            uiKernel.SetScoreInfo(kernelInfo);

            RectTransform kernelRect = lastAnchor = uiKernel.Rect;

            kernelRect.SetParent(anchorStart);
            kernelRect.SetAsFirstSibling();

            Vector3 pos = kernelRect.localPosition;
            pos.y = (i_kernels.Count % 2 == 0 ? -2.5f : 2.5f);
            kernelRect.localPosition = pos;

            uiKernelQueue.Enqueue(uiKernel);

            StartCoroutine(
                scaleRoutine(uiKernel.transform, interpolatorsManager.Animate(
                MathConstants.VECTOR_3_ONE,
                MathConstants.VECTOR_3_ONE * 1.5f,
                0.5f,
                new AnimationMode(AnimationType.Bounce))));

        }

        if (null == dequeueKernelsRoutine) dequeueKernelsRoutine = StartCoroutine(dequeueKernels());
    }

    #endregion

    #region PRIVATE

    float getTimePerUIKernel()
    {
        float time = Mathf.Lerp(timePerUIKernel, timePerUIKernelFrenzy, (float)uiKernelQueue.Count / 20f);
        return time;
    }

    IEnumerator scaleRoutine(Transform i_tr, ITypedAnimator<Vector3> i_scaleAnimator)
    {
        while (true == i_scaleAnimator.IsAnimating)
        {
            i_tr.localScale = i_scaleAnimator.Current;
            yield return null;
        }
    }

    IEnumerator dequeueKernels()
    {
        yield return this.Wait(getTimePerUIKernel() * 2f);

        while (uiKernelQueue.Count != 0)
        {
            UIDoraKernel uiKernel = uiKernelQueue.Dequeue();
            ScoreKernelInfo scoreKernelInfo = uiKernel.ScoreInfo;

            float timeBase = getTimePerUIKernel();

            sfxProvider.PlayUIKernelSFX(scoreKernelInfo.KernelStatus);
            yield return StartCoroutine(animateKernel(uiKernel, timeBase));

           // Debug.Break();

            uiKernelSpawner.DespawnKernel(uiKernel);

            scoreManager.AddScoreByStatus(scoreKernelInfo,
                                           anchorScore,
                                           timeBase * 2f, 0.1f, -100f);

            StartCoroutine(
                scaleRoutine(scoreManager.ScoreRect, interpolatorsManager.Animate(
                MathConstants.VECTOR_3_ONE,
                MathConstants.VECTOR_3_ONE * 1.075f,
                timeBase,
                new AnimationMode(AnimationType.Bounce))));


            yield return StartCoroutine(shitftKernelStack(timeBase));

           // Debug.Break();

        }

        anchorStart.anchoredPosition = anchorStartInitialAnchoredPosition;
        lastAnchor = null;

        this.DisposeCoroutine(ref dequeueKernelsRoutine);
    }

    IEnumerator animateKernel(UIDoraKernel i_uiKernel, float i_time)
    {
        i_uiKernel.transform.SetParent(anchorStart.parent);

        UIElementMove elementMove = i_uiKernel.GetComponent<UIElementMove>();
        elementMove.MoveToPosition(new Vector2(anchorEnd.position.x, i_uiKernel.transform.position.y), false, i_time, interpolatorsManager, positionCurve, null);

        UIElementAlpha elementAlpha = i_uiKernel.GetComponent<UIElementAlpha>();
        elementAlpha.lerpAlpha(1f, 0f, i_time, interpolatorsManager, alphaCurve, null);

        StartCoroutine(
            scaleRoutine(i_uiKernel.transform, interpolatorsManager.Animate(
            MathConstants.VECTOR_3_ONE,
            MathConstants.VECTOR_3_ONE * 1.8f,
            i_time / 2f,
            new AnimationMode(AnimationType.Ease_In_Out))));

        while (true == elementMove.IsAnimated) yield return null;
    }

    IEnumerator shitftKernelStack(float i_time)
    {
        UIElementMove stackMove = anchorStart.GetComponent<UIElementMove>();
        stackMove.MoveToPosition(anchorStart.position + MathConstants.VECTOR_3_LEFT * xOffsetPerUIKernel, false, i_time / 2f, interpolatorsManager, stackShiftCurve, null);

        while(true == stackMove.IsAnimated) yield return null;
    }

    #endregion












    /* [ExposePublicMethod]
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
                             i_eatenKernels[i].KernelStatus == KernelStatus.Burnt));
         }

         for (int i = 0; i < length; i++)
         {
             currKernelInfo = i_eatenKernels[i];
             //some animation stuff
             yield return StartCoroutine(uiKernelMovementSequence(uiKernels[i], timePerUIKernel));

             scoreManager.AddScoreByStatus(currKernelInfo.KernelStatus,
                                           currKernelInfo.ScoreMultiplier,
                                           anchorScore,
                                           timePerUIKernel / 2, 0.01f, 10f);
         }

         Debug.LogError("done with kernel stack");

         this.DisposeCoroutine(ref kernelStackRoutine);
     }

     private IEnumerator uiKernelMovementSequence(RectTransform i_uiKernel, float i_timeAvailable)
     {
         UIElementMove elementMove = i_uiKernel.GetComponent<UIElementMove>();
         //Debug.LogError("try element move");

         if (elementMove != null)
         {
             //Debug.LogError("element move");
             elementMove.moveToRectTransform(anchorEnd, i_timeAvailable, interpolatorsManager, animCurve, null);
         }
         yield return this.Wait(i_timeAvailable * 0.8f);

         UIElementAlpha elementAlpha = i_uiKernel.GetComponent<UIElementAlpha>();
         if (elementAlpha != null)
             elementAlpha.lerpAlpha(1f, 0f, i_timeAvailable * 0.2f, interpolatorsManager, animCurve, null);
         yield return this.Wait(i_timeAvailable * 0.2f);

         uiKernelSpawner.DespawnKernel(i_uiKernel);
     } */


}
