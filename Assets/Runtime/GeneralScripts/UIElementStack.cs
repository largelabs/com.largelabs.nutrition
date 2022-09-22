using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIElementStack<T> : MonoBehaviourBase
{
    [Header("Stack Configs")]
    [SerializeField] protected RectTransform anchorStart = null;
    [SerializeField] protected RectTransform anchorEnd = null;
    [SerializeField] protected RectTransform anchorScore = null;
    [SerializeField] protected float xOffsetPerUIKernel = -60.0f;
    [SerializeField] protected InterpolatorsManager interpolatorsManager = null;
    [SerializeField] private AnimationCurve alphaCurve = null;
    [SerializeField] private AnimationCurve positionCurve = null;
    [SerializeField] private AnimationCurve stackShiftCurve = null;
    [SerializeField] protected bool shiftStackInDirectionOfOffset = false;
    [SerializeField] protected bool autoDequeue = true;

    [Header("Animation timing")]
    [SerializeField] bool scaleAnimationTime = false;
    [SerializeField] int queueSizeScalingRef = 20;
    [SerializeField] protected float timePerUIElement = 0.2f;
    [SerializeField] protected float minTimePerUIElement = 0.2f;

    protected RectTransform lastAnchor = null;
    protected Vector3 anchorStartInitialAnchoredPosition = MathConstants.VECTOR_3_ZERO;
    Vector3 anchorStartInitialPosition = MathConstants.VECTOR_3_ZERO;

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
    public abstract void CollectUIElements(Queue<T> i_kernels);
    #endregion

    #region PROTECTED API
    protected IEnumerator scaleRoutine(Transform i_tr, ITypedAnimator<Vector3> i_scaleAnimator)
    {
        while (true == i_scaleAnimator.IsAnimating)
        {
            i_tr.localScale = i_scaleAnimator.Current;
            yield return null;
        }
    }

    protected abstract IEnumerator discardUIElements();

    protected IEnumerator animateElement(GameObject i_uiElement)
    {
        i_uiElement.transform.SetParent(anchorStart.parent);

        UIElementMove elementMove = i_uiElement.GetComponent<UIElementMove>();
        if (elementMove == null)
        {
            Debug.LogError("Missing componenet element move!");
            yield break;
        }
        elementMove.MoveToPosition(new Vector2(anchorEnd.position.x, i_uiElement.transform.position.y), false, getTimePerUIElement(), interpolatorsManager, positionCurve, null);

        UIElementAlpha elementAlpha = i_uiElement.GetComponent<UIElementAlpha>();
        if (elementAlpha == null)
        {
            Debug.LogError("Missing componenet element alpha!");
            yield break;
        }
        elementAlpha.lerpAlpha(1f, 0f, getTimePerUIElement(), interpolatorsManager, alphaCurve, null);

        StartCoroutine(
            scaleRoutine(i_uiElement.transform, interpolatorsManager.Animate(
            MathConstants.VECTOR_3_ONE,
            MathConstants.VECTOR_3_ONE * 1.8f,
            getTimePerUIElement() / 2f,
            new AnimationMode(AnimationType.Ease_In_Out))));

        while (true == elementMove.IsAnimated) yield return null;
    }  
    
    protected IEnumerator animateElement(GameObject i_uiElement, 
        bool i_animatePos, 
        bool i_animateAlpha, 
        bool i_animateScale)
    {
        UIElementMove elementMove = null;
        UIElementAlpha elementAlpha = null;

        if (i_animatePos)
        {
            i_uiElement.transform.SetParent(anchorStart.parent);

            elementMove = i_uiElement.GetComponent<UIElementMove>();
            if (elementMove == null)
            {
                Debug.LogError("Missing componenet element move!");
                yield break;
            }
            elementMove.MoveToPosition(new Vector2(anchorEnd.position.x, i_uiElement.transform.position.y), false, getTimePerUIElement(), interpolatorsManager, positionCurve, null);

        }

        if (i_animateAlpha)
        {
            elementAlpha = i_uiElement.GetComponent<UIElementAlpha>();
            if (elementAlpha == null)
            {
                Debug.LogError("Missing componenet element alpha!");
                yield break;
            }
            elementAlpha.lerpAlpha(1f, 0f, getTimePerUIElement(), interpolatorsManager, alphaCurve, null);
        }

        if (i_animateScale)
        {
            yield return StartCoroutine(
                            scaleRoutine(i_uiElement.transform, interpolatorsManager.Animate(
                            MathConstants.VECTOR_3_ONE,
                            MathConstants.VECTOR_3_ONE * 1.8f,
                            getTimePerUIElement() / 2f,
                            new AnimationMode(AnimationType.Ease_In_Out))));
        }


        while ((elementMove != null && true == elementMove.IsAnimated) ||
                (elementAlpha != null && true == elementAlpha.IsAnimated))
            yield return null;
    }

    protected IEnumerator shiftElementStack()
    {
        UIElementMove stackMove = anchorStart.GetComponent<UIElementMove>();
        Vector3 shiftDirection = (shiftStackInDirectionOfOffset) ? MathConstants.VECTOR_3_RIGHT : MathConstants.VECTOR_3_LEFT;
        stackMove.MoveToPosition(anchorStart.position + shiftDirection * xOffsetPerUIKernel, true, getTimePerUIElement() / 2f, interpolatorsManager, stackShiftCurve, null);

        while (true == stackMove.IsAnimated) yield return null;
    }

    protected virtual int currentStackSize => 0;

    protected virtual float getTimePerUIElement()
    {
        if (false == scaleAnimationTime) return timePerUIElement;

        float time = Mathf.Lerp(timePerUIElement, minTimePerUIElement, (float)currentStackSize / (float)queueSizeScalingRef);
        return time;
    }

    #endregion
}
