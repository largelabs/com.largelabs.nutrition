﻿
using System.Collections;
using UnityEngine;

public class DoraRaycastController : DoraAbstractController
{
    [Header("Raycast Controller Settings")]
    [SerializeField] float minLocalX = -6.5f;
    [SerializeField] float maxLocalX = 5f;
    [SerializeField] float raycastSourceMoveSpeed = 200f;
    [SerializeField] DoraSelectionRaycastSource raycastSource = null;
    [SerializeField] UIDoraRaycastPointer pointerUI = null;

    Coroutine rayCastRoutine = null;
    Coroutine autoMoveRoutine = null;
    Coroutine centerSourceRoutine = null;
    ITypedAnimator<Vector3> moveInterpolator = null;

    Vector3 targetPos = Vector3.zero;
    float autoMoveSpeed = 1f;

    #region UNITY & CORE

    protected override void Start()
    {
        base.Start();
        targetPos = new Vector3(maxLocalX, raycastSource.transform.localPosition.y, raycastSource.transform.localPosition.z);
    }
    #endregion

    #region PUBLIC API

    public override void StartAutoRotation()
    {
        base.StartAutoRotation();
        if (null == rayCastRoutine) rayCastRoutine = StartCoroutine(updateRaycast());
    }

    public override void StopAutoRotation()
    {
        base.StopAutoRotation();
        this.DisposeCoroutine(ref rayCastRoutine);
    }

    public void StartAutoMove(float i_speed)
    {
        autoMoveSpeed = i_speed;
        setInterpolationTarget();

        startPointerAutoMove();
    }

    public void StopAutoMove()
    {
        this.DisposeCoroutine(ref autoMoveRoutine);
        interpolators.Stop(moveInterpolator);
    }

    #endregion

    #region PROTECTED

    protected override void enableControllerUI(bool i_enable)
    {
        pointerUI.EnablePointer(i_enable);
    }

    protected override void move(Vector2 i_move)
    {
        Vector3 pos = raycastSource.transform.localPosition;
        pos.x += Time.deltaTime * raycastSourceMoveSpeed * (-i_move.x);
        pos.x = Mathf.Clamp(pos.x, minLocalX, maxLocalX);
        raycastSource.transform.localPosition = pos;
    }

    protected override void onEatStarted()
    {
        base.onEatStarted();
        this.DisposeCoroutine(ref centerSourceRoutine);
    }

    protected override void onEat()
    {
        if (false == IsSelectingKernel()) return;
        if (false == didStartEat) return;

        Vector2Int? currentSelect = cellSelector.CurrentOriginCell;
        if (null == currentSelect) return;

        if (false == IsInFrenzy && null == centerSourceRoutine)
        {
            DoraCellData cell = cellMap.GetCell(currentSelect.Value, false, false);
            centerSourceRoutine = StartCoroutine(recenterPointer(cell.Anchor));
        }

        base.onEat();
    }

    protected override void onEatReleased()
    {
        this.DisposeCoroutine(ref centerSourceRoutine);
        base.onEatReleased();
    }

    #endregion

    #region PRIVATE

    IEnumerator recenterPointer(Transform i_anchor)
    {
        Vector3 source = raycastSource.transform.position;
        Vector3 target = i_anchor.position;
        target.y = source.y;

        ITypedAnimator<Vector3> interpolator = interpolators.Animate(
            source,
            target,
            0.1f,
            new AnimationMode(AnimationType.Ease_In_Out));

        while(true == interpolator.IsActive)
        {
            Vector2 current = interpolator.Current;
            Vector3 pos = raycastSource.transform.position;
            pos.x = current.x;
            pos.y = current.y;
            raycastSource.transform.position = pos;
            yield return null;
        }
    }

    IEnumerator updateRaycast()
    {
        while (true)
        {
            DoraCellData cellData = cellMap.GetCell(null != raycastSource ? raycastSource.HitGo : null);

            if (null != cellData)
            {
                bool clearSelection = 
                    null != cellSelector.CurrentOriginCell 
                    && cellSelector.CurrentOriginCell.Value != cellData.Coords;


                if (false == IsInFrenzy) 
                    cellSelector.SelectCell(cellData.Coords, false, clearSelection);
                else
                    cellSelector.SelectRange(cellData.Coords, DoraGameplayData.FrenzySelectionRange, true, false, true);
            }
            else
                cellSelector.ClearSelection();

            yield return null;
        }
    }

    private void startPointerAutoMove()
    {
        StopAutoMove();

        float time = (targetPos - raycastSource.transform.localPosition).magnitude / autoMoveSpeed;

        moveInterpolator = interpolators.Animate(raycastSource.transform.localPosition, targetPos, time, new AnimationMode(AnimationType.Ease_In_Out), false, 0f);

        autoMoveRoutine = StartCoroutine(autoMove(moveInterpolator));
    }

    private IEnumerator autoMove(ITypedAnimator<Vector3> i_moveInterpolator)
    {

        while (true == i_moveInterpolator.IsActive)
        {
            raycastSource.transform.localPosition = i_moveInterpolator.Current;
            yield return null;
        }

        setInterpolationTarget();

        startPointerAutoMove();
    }

    private void setInterpolationTarget()
    {
        if (Mathf.Abs(raycastSource.transform.localPosition.x - minLocalX) > Mathf.Abs(raycastSource.transform.localPosition.x - maxLocalX)) targetPos.x = minLocalX;
        else targetPos.x = maxLocalX;
    }
    #endregion
}

