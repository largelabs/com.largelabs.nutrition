using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHarrankashStack : UIElementStack<float>
{
    [SerializeField] UIHarrankashSpawner uiHarrankashSpawner = null;
    [SerializeField] HarraSFXProvider sfxProvider = null;

    //score and sfx stuff is commented until further notice
    //[SerializeField] DoraScoreManager scoreManager = null;
    //[SerializeField] DoraSFXProvider sfxProvider = null;

    public Action OnDiscardHarrankash = null;

    private Queue<float> queuedHarrankash = null;
    private Stack<UIImageFrameSwapper> uiHarrankashStack = null;
    private Stack<float> scoreStack = null;

    Coroutine stackingRoutine = null;

    #region UNITY
    protected override void Awake()
    {
        base.Awake();

        if (stackingRoutine == null)
            stackingRoutine = StartCoroutine(QueueToStack());
    }

    private void OnDestroy()
    {
        this.DisposeCoroutine(ref stackingRoutine);
    }

    #endregion

    #region PUBLIC API
    public bool IsDestacking => dequeueKernelsRoutine != null;

    protected override int currentStackSize => uiHarrankashStack == null ? 0 : uiHarrankashStack.Count;

    public override void CollectUIElements(Queue<float> i_platformScores)
    {
        while (i_platformScores.Count > 0)
            queuedHarrankash.Enqueue(i_platformScores.Dequeue());
    }

    public void DestackHarrankash()
    {
        if (null == dequeueKernelsRoutine) 
            dequeueKernelsRoutine = StartCoroutine(discardUIElements());
    }
    #endregion

    #region PROTECTED API
    protected override IEnumerator discardUIElements()
    {
        //yield return this.Wait(base.getTimePerUIElement() * 2f);

        while (uiHarrankashStack.Count != 0)
        {
            UIImageFrameSwapper uiHarraAnimation = uiHarrankashStack.Pop();

            // sfx suggestion: collection sound for counting each UI harankash
            sfxProvider.PlayStackSFX();

            yield return StartCoroutine(animateElement(uiHarraAnimation.gameObject, true, true, false));

            uiHarrankashSpawner.DespawnTransform(uiHarraAnimation);
            OnDiscardHarrankash?.Invoke();

        }

        anchorStart.anchoredPosition = anchorStartInitialAnchoredPosition;
        lastAnchor = null;

        this.DisposeCoroutine(ref dequeueKernelsRoutine);
    }
    #endregion

    #region PRIVATE
    private IEnumerator QueueToStack()
    {
        if (null == lastAnchor)
        {
            lastAnchor = anchorStart;
        }

        if (null == uiHarrankashStack) uiHarrankashStack = new Stack<UIImageFrameSwapper>();
        if (null == scoreStack) scoreStack = new Stack<float>();

        if (queuedHarrankash == null) queuedHarrankash = new Queue<float>();

        while (true)
        {
            while (queuedHarrankash.Count > 0)
            {
                scoreStack.Push(queuedHarrankash.Dequeue());
                UIImageFrameSwapper uiHarraAnimation = 
                    uiHarrankashSpawner.SpawnTransformAtAnchor(lastAnchor, 
                    new Vector3(xOffsetPerUIKernel, 0, 0), HarraEnumReference.UIHarrankashTypes.Orange,
                    true, true, false);

                RectTransform kernelRect = lastAnchor = uiHarraAnimation.transform as RectTransform;

                kernelRect.SetParent(anchorStart);
                kernelRect.SetAsFirstSibling();

                uiHarrankashStack.Push(uiHarraAnimation);

                Vector3 pos = kernelRect.localPosition;
                pos.y = (uiHarrankashStack.Count % 2 == 0 ? -2.5f : 2.5f);
                kernelRect.localPosition = pos;

                yield return StartCoroutine(
                    scaleRoutine(uiHarraAnimation.transform, interpolatorsManager.Animate(
                    uiHarraAnimation.transform.localScale,
                    uiHarraAnimation.transform.localScale * 1.5f,
                    0.5f,
                    new AnimationMode(AnimationType.Bounce))));
            }

            if(autoDequeue && uiHarrankashStack.Count > 0)
                yield return StartCoroutine(discardUIElements());

           yield return null;
        }
    }
    #endregion
}
