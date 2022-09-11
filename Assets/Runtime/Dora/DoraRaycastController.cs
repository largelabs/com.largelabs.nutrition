
using System.Collections;
using UnityEngine;

public class DoraRaycastController : DoraAbstractController
{
    [Header("Raycast Controller Settings")]
    [SerializeField] float minLocalX = -6.5f;
    [SerializeField] float maxLocalX = 5f;
    [SerializeField] float raycastSourceMoveSpeed = 200f;
    [SerializeField] float raycastAutoMoveSpeed = 20f;
    [SerializeField] DoraSelectionRaycastSource raycastSource = null;
    [SerializeField] InterpolatorsManager interpolators = null;

    Coroutine rayCastRoutine = null;

    ITypedAnimator<Vector3> moveInterpolator = null;
    Vector3 targetPos = Vector3.zero;
    float autoMoveTime = 1f;

    #region UNITY & CORE

    private void Update()
    {
        if (null != moveInterpolator && true == moveInterpolator.IsActive) raycastSource.transform.localPosition = moveInterpolator.Current;
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

    public void StartAutoMove(float i_time)
    {
        targetPos = new Vector3(maxLocalX, raycastSource.transform.localPosition.y, raycastSource.transform.localPosition.z);
        autoMoveTime = i_time;
        autoMove();
    }

    public void StopAutoMove()
    {
        moveInterpolator = null;
    }

    #endregion


    #region PROTECTED

    protected override void move(Vector2 i_move)
    {
        Vector3 pos = raycastSource.transform.localPosition;
        pos.x += Time.deltaTime * raycastSourceMoveSpeed * (-i_move.x);
        pos.x = Mathf.Clamp(pos.x, minLocalX, maxLocalX);
        raycastSource.transform.localPosition = pos;
    }

    #endregion

    #region PRIVATE

    IEnumerator updateRaycast()
    {
        while (true)
        {
            DoraCellData cellData = cellMap.GetCell(null != raycastSource ? raycastSource.HitGo : null);

            if (null != cellData)
            {
                bool clearSelection = null != cellSelector.CurrentOriginCell && cellSelector.CurrentOriginCell.Value != cellData.Coords;
                cellSelector.SelectCell(cellData.Coords, false, clearSelection);
            }
            else
                cellSelector.ClearSelection();

            yield return null;
        }
    }

    private void autoMove()
    {
        moveInterpolator = interpolators.Animate(raycastSource.transform.localPosition, targetPos, autoMoveTime, new AnimationMode(AnimationType.Linear), false, 0.2f, onMoveAnimationEnded);
    }

    private void onMoveAnimationEnded(ITypedAnimator<Vector3> i_interpolator)
    {
        if (targetPos.x == minLocalX) targetPos.x = maxLocalX;
        else targetPos.x = minLocalX;

        if (moveInterpolator != null) autoMove();
    }

    #endregion
}

