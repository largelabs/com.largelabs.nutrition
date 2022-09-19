using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIKernelManagerV2 : UIElementStack<UIDoraKernel, ScoreKernelInfo>
{
    [SerializeField] DoraScoreManager scoreManager = null;
    [SerializeField] UIKernelSpawner uiKernelSpawner = null;
    [SerializeField] private float timePerUIKernelFrenzy = 0.2f;
    [SerializeField] DoraSFXProvider sfxProvider = null;

    bool isFrenzyActive = false;

    #region PUBLIC API

    public void ActivateFrenzy(bool i_active)
    {
        isFrenzyActive = i_active;
    }

    public override void EnqueueKernels(Queue<ScoreKernelInfo> i_kernels)
    {
        if (null == lastAnchor)
        {
            lastAnchor = anchorStart;
        }

        if (null == uiElementQueue) uiElementQueue = new Queue<UIDoraKernel>();

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

            uiElementQueue.Enqueue(uiKernel);

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
    protected override IEnumerator dequeueKernels()
    {
        yield return this.Wait(base.getTimePerUIElement() * 2f);

        while (uiElementQueue.Count != 0)
        {
            UIDoraKernel uiKernel = uiElementQueue.Dequeue();
            ScoreKernelInfo scoreKernelInfo = uiKernel.ScoreInfo;

            sfxProvider.PlayUIKernelSFX(scoreKernelInfo.KernelStatus);
            yield return StartCoroutine(animateKernel(uiKernel));


            uiKernelSpawner.DespawnKernel(uiKernel);

            scoreManager.AddScoreByStatus(scoreKernelInfo,
                                           anchorScore,
                                           base.getTimePerUIElement() * 2f, 0.1f, -100f);

            StartCoroutine(
                scaleRoutine(scoreManager.ScoreRect, interpolatorsManager.Animate(
                MathConstants.VECTOR_3_ONE,
                MathConstants.VECTOR_3_ONE * 1.075f,
                getTimePerUIElement(),
                new AnimationMode(AnimationType.Bounce))));


            yield return StartCoroutine(shitftKernelStack());
        }

        anchorStart.anchoredPosition = anchorStartInitialAnchoredPosition;
        lastAnchor = null;

        this.DisposeCoroutine(ref dequeueKernelsRoutine);
    }

    protected override float getTimePerUIElement()
    {
        return isFrenzyActive ? timePerUIKernelFrenzy : timePerUIElement;
    }
    #endregion
}
