
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
    Coroutine autoMoveRoutine = null;
    ITypedAnimator<Vector3> moveInterpolator = null;

    Vector3 targetPos = Vector3.zero;
    float autoMoveSpeed = 1f;

    #region PUBLIC API

    public override void StartAutoRotation(bool i_setDefaultSpeed)
    {
        base.StartAutoRotation(i_setDefaultSpeed);
        if (null == rayCastRoutine) rayCastRoutine = StartCoroutine(updateRaycast());
    }

    public override void StopAutoRotation()
    {
        base.StopAutoRotation();
        this.DisposeCoroutine(ref rayCastRoutine);
    }

    public void StartAutoMove(float i_speed)
    {
        Debug.LogError("Start AutoMove");
        targetPos = new Vector3(maxLocalX, raycastSource.transform.localPosition.y, raycastSource.transform.localPosition.z);
        autoMoveSpeed = i_speed;
        startAutoMove();
    }

    public void StopAutoMove()
    {
        Debug.LogError("Stop AutoMove");

        this.DisposeCoroutine(ref autoMoveRoutine);
        interpolators.Stop(moveInterpolator);


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

                
                if(null == frenzyRoutine) cellSelector.SelectCell(cellData.Coords, false, clearSelection);
                else 
                    cellSelector.SelectRange(cellData.Coords, 2, true, false, true); 
            }
            else
                cellSelector.ClearSelection();

            yield return null;
        }
    }

    private void startAutoMove()
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

        if (targetPos.x == minLocalX) targetPos.x = maxLocalX;
        else targetPos.x = minLocalX;

        startAutoMove();
    }
    #endregion
}

