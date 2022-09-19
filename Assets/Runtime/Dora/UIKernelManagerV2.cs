using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIKernelManagerV2 : UIElementStack<ScoreKernelInfo>
{
    [SerializeField] DoraScoreManager scoreManager = null;
    [SerializeField] UIKernelSpawner uiKernelSpawner = null;
    [SerializeField] float timePerUIKernelFrenzy = 0.2f;
    [SerializeField] DoraSFXProvider sfxProvider = null;

    Queue<UIDoraKernel> uiKernelQueue = null;

    bool isFrenzyActive = false;

    #region PUBLIC API

    public void ActivateFrenzy(bool i_active)
    {
        isFrenzyActive = i_active;
    }

    public override void CollectUIElements(Queue<ScoreKernelInfo> i_kernels)
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

        if (null == dequeueKernelsRoutine) dequeueKernelsRoutine = StartCoroutine(discardUIElements());
    }

    #endregion

    #region PRIVATE
    protected override IEnumerator discardUIElements()
    {
        yield return this.Wait(base.getTimePerUIElement() * 2f);

        while (uiKernelQueue.Count != 0)
        {
            UIDoraKernel uiKernel = uiKernelQueue.Dequeue();
            ScoreKernelInfo scoreKernelInfo = uiKernel.ScoreInfo;

            sfxProvider.PlayUIKernelSFX(scoreKernelInfo.KernelStatus);
            yield return StartCoroutine(animateKernel(uiKernel.gameObject));


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
