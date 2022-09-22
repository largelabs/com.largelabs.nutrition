using System;
using System.Collections;
using UnityEngine;

public class PositionAnimator : MonoBehaviourBase
{
    [SerializeField] Transform transformToMove = null;
    [SerializeField] InterpolatorsManager interpolatorsManager = null;
    [SerializeField] Vector3 startPosition = Vector3.zero;
    [SerializeField] Vector3 endPosition = Vector3.zero;
    [SerializeField] Transform startTransform = null;
    [SerializeField] Transform endTransform = null;
    [SerializeField] bool useTransformAsDefault = true;
    [SerializeField] float animationTime = 0f;
    [SerializeField] bool clampValues = true;
    [SerializeField] AnimationCurve animationCurve = null;

    Coroutine movementRoutine = null;

    // local values
    Vector3 startPos;
    Vector3 endPos;
    bool clamp;
    float time;
    InterpolatorsManager interps;
    AnimationCurve curve;
    Action<ITypedAnimator<Vector3>> callback;

    #region UNITY AND CORE
    protected override void Awake()
    {
        base.Awake();

        setAnimatorValues();
    }

    private void OnDisable()
    {
        this.DisposeCoroutine(ref movementRoutine);
    }

    private void OnDestroy()
    {
        this.DisposeCoroutine(ref movementRoutine);
    }

    #endregion

    #region PUBLIC API
    public bool IsMoving => movementRoutine != null;

    public void MoveToPosition(Vector3? i_startPos, Vector3? i_endPos, bool? i_clamp, float? i_time,
                                InterpolatorsManager i_interps, AnimationCurve i_curve,
                                Action<ITypedAnimator<Vector3>> i_callback)
    {
        setAnimatorValues(i_startPos, i_endPos, i_clamp, i_time, i_interps, i_curve, i_callback);

        if (interps == null)
        {
            Debug.LogError("Invalid interpolator manager passed! Returning...");
            return;
        }

        if (movementRoutine == null)
        {
            movementRoutine = StartCoroutine(movementSequence());
        }
    }
    
    public void MoveToPosition()
    {
        setAnimatorValues();

        if (interps == null)
        {
            Debug.LogError("Invalid interpolator manager assigned! Returning...");
            return;
        }

        if (movementRoutine == null)
        {
            movementRoutine = StartCoroutine(movementSequence());
        }
    }

    private void setAnimatorValues(Vector3? i_startPos, Vector3? i_endPos, bool? i_clamp, float? i_time,
                                InterpolatorsManager i_interps, AnimationCurve i_curve,
                                Action<ITypedAnimator<Vector3>> i_callback)
    {
        startPos = i_startPos == null? (useTransformAsDefault ? startTransform.position : startPosition) :i_startPos.Value;
        endPos = i_endPos == null? (useTransformAsDefault ? endTransform.position : endPosition) :i_endPos.Value;
        clamp =  i_clamp == null? clampValues:i_clamp.Value;
        time = i_time == null? animationTime:i_time.Value;
        interps = i_interps == null? interpolatorsManager:i_interps;
        curve = i_curve == null? animationCurve:i_curve;
        callback = i_callback;
    } 
    
    private void setAnimatorValues()
    {
        startPos = useTransformAsDefault ? startTransform.position:startPosition;
        endPos = useTransformAsDefault ? endTransform.position:endPosition;
        clamp = clampValues;
        time = animationTime;
        interps = interpolatorsManager;
        curve = animationCurve;
        callback = null;
    }

    #endregion

    #region PRIVATE

    private IEnumerator movementSequence()
    {
        AnimationMode mode = new AnimationMode(curve);
        ITypedAnimator<Vector3> posInterpolator = interps.Animate(startPos, endPos, time, mode, clamp, 0f, callback);

        if (transformToMove == null)
        {
            Debug.LogError("No transform assigned, defaulting to attached transform...");
            transformToMove = this.transform;
        }


        while (posInterpolator.IsActive)
        {
            transformToMove.position = posInterpolator.Current;
            yield return null;
        }

        this.DisposeCoroutine(ref movementRoutine);
    }

    #endregion
}
