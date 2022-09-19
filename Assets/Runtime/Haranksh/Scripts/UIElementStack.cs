using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIElementStack<T1, T2> : MonoBehaviourBase
{
    [Header("Stack Configs")]
    [SerializeField] protected RectTransform anchorStart = null;
    [SerializeField] protected RectTransform anchorEnd = null;
    [SerializeField] protected RectTransform anchorScore = null;
    [SerializeField] protected float timePerUIElement = 0.2f;
    [SerializeField] protected float xOffsetPerUIKernel = -60.0f;
    [SerializeField] protected InterpolatorsManager interpolatorsManager = null;
    [SerializeField] private AnimationCurve alphaCurve = null;
    [SerializeField] private AnimationCurve positionCurve = null;
    [SerializeField] private AnimationCurve stackShiftCurve = null;

    protected RectTransform lastAnchor = null;
    protected Vector3 anchorStartInitialAnchoredPosition = MathConstants.VECTOR_3_ZERO;
    Vector3 anchorStartInitialPosition = MathConstants.VECTOR_3_ZERO;

    protected Queue<T1> uiElementQueue = null;

    protected Coroutine dequeueKernelsRoutine = null;

    #region UNITY AND CORE
    protected override void Awake()
    {
        base.Awake();
        anchorStartInitialAnchoredPosition = anchorStart.anchoredPosition;
        anchorStartInitialPosition = anchorStart.position;
    }
    #endregion

    #region PUBLIC API
    public abstract void EnqueueKernels(Queue<T2> i_kernels);
    #endregion

    #region PRIVATE
    protected IEnumerator scaleRoutine(Transform i_tr, ITypedAnimator<Vector3> i_scaleAnimator)
    {
        while (true == i_scaleAnimator.IsAnimating)
        {
            i_tr.localScale = i_scaleAnimator.Current;
            yield return null;
        }
    }

    protected abstract IEnumerator dequeueKernels();

    protected IEnumerator animateKernel(UIDoraKernel i_uiKernel)
    {
        i_uiKernel.transform.SetParent(anchorStart.parent);

        UIElementMove elementMove = i_uiKernel.GetComponent<UIElementMove>();
        elementMove.MoveToPosition(new Vector2(anchorEnd.position.x, i_uiKernel.transform.position.y), false, getTimePerUIElement(), interpolatorsManager, positionCurve, null);

        UIElementAlpha elementAlpha = i_uiKernel.GetComponent<UIElementAlpha>();
        elementAlpha.lerpAlpha(1f, 0f, getTimePerUIElement(), interpolatorsManager, alphaCurve, null);

        StartCoroutine(
            scaleRoutine(i_uiKernel.transform, interpolatorsManager.Animate(
            MathConstants.VECTOR_3_ONE,
            MathConstants.VECTOR_3_ONE * 1.8f,
            getTimePerUIElement() / 2f,
            new AnimationMode(AnimationType.Ease_In_Out))));

        yield return this.Wait(getTimePerUIElement());
    }

    protected IEnumerator shitftKernelStack()
    {
        UIElementMove stackMove = anchorStart.GetComponent<UIElementMove>();
        stackMove.MoveToPosition(anchorStart.position + MathConstants.VECTOR_3_LEFT * xOffsetPerUIKernel, true, getTimePerUIElement() / 2f, interpolatorsManager, stackShiftCurve, null);

        yield return this.Wait(getTimePerUIElement() / 2f);

    }

    protected virtual float getTimePerUIElement()
    {
        return timePerUIElement;
    }
    #endregion
}
