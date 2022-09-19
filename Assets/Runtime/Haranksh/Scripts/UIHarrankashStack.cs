using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHarrankashStack : UIElementStack<float>
{
    [SerializeField] UIHarrankashSpawner uiHarrankashSpawner = null;

    //score and sfx stuff is commented until further notice
    //[SerializeField] DoraScoreManager scoreManager = null;
    //[SerializeField] DoraSFXProvider sfxProvider = null;

    private Stack<UIImageFrameSwapper> uiHarrankashStack = null;
    private Stack<float> scoreStack = null;

    #region PUBLIC API
    public override void CollectUIElements(Queue<float> i_platformScores)
    {
        if (null == lastAnchor)
        {
            lastAnchor = anchorStart;
        }

        if (null == uiHarrankashStack) uiHarrankashStack = new Stack<UIImageFrameSwapper>();
        if (null == scoreStack) scoreStack = new Stack<float>();

        while (i_platformScores.Count > 0)
        {
            scoreStack.Push(i_platformScores.Dequeue());
            UIImageFrameSwapper uiHarraAnimation = uiHarrankashSpawner.SpawnTransformAtAnchor(lastAnchor, new Vector3(xOffsetPerUIKernel,0, 0), UIHarrankashTypes.Orange);

            RectTransform kernelRect = lastAnchor = uiHarraAnimation.transform as RectTransform;

            kernelRect.SetParent(anchorStart);
            kernelRect.SetAsFirstSibling();

            Vector3 pos = kernelRect.localPosition;
            pos.y = (i_platformScores.Count % 2 == 0 ? -2.5f : 2.5f);
            kernelRect.localPosition = pos;

            uiHarrankashStack.Push(uiHarraAnimation);

            StartCoroutine(
                scaleRoutine(uiHarraAnimation.transform, interpolatorsManager.Animate(
                MathConstants.VECTOR_3_ONE,
                MathConstants.VECTOR_3_ONE * 1.5f,
                0.5f,
                new AnimationMode(AnimationType.Bounce))));

        }

        if (null == dequeueKernelsRoutine) dequeueKernelsRoutine = StartCoroutine(discardUIElements());
    }
    #endregion

    #region PROTECTED API
    protected override IEnumerator discardUIElements()
    {
        yield return this.Wait(base.getTimePerUIElement() * 2f);

        while (uiHarrankashStack.Count != 0)
        {
            UIImageFrameSwapper uiHarraAnimation = uiHarrankashStack.Pop();

            //sfxProvider.PlayUIKernelSFX(scoreKernelInfo.KernelStatus);
            yield return StartCoroutine(animateKernel(uiHarraAnimation.gameObject));


            uiHarrankashSpawner.DespawnTransform(uiHarraAnimation);

            //scoreManager.AddScoreByStatus(scoreKernelInfo,
            //                               anchorScore,
            //                               base.getTimePerUIElement() * 2f, 0.1f, -100f);

            //StartCoroutine(
            //    scaleRoutine(scoreManager.ScoreRect, interpolatorsManager.Animate(
            //    MathConstants.VECTOR_3_ONE,
            //    MathConstants.VECTOR_3_ONE * 1.075f,
            //    getTimePerUIElement(),
            //    new AnimationMode(AnimationType.Bounce))));


            yield return StartCoroutine(shitftKernelStack());
        }

        anchorStart.anchoredPosition = anchorStartInitialAnchoredPosition;
        lastAnchor = null;

        this.DisposeCoroutine(ref dequeueKernelsRoutine);
    }
    #endregion

    #region PRIVATE

    #endregion
}
