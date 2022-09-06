﻿
using System.Collections;
using UnityEngine;

public class DoraRaycastController : DoraController
{
    [SerializeField] float minLocalX = -6.5f;
    [SerializeField] float maxLocalX = 5f;
    [SerializeField] float raycastSourceMoveSpeed = 200f;
    [SerializeField] DoraSelectionRaycastSource raycastSource = null;

    Coroutine rayCastRoutine = null;

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
               // cellSelector.SelectRange(cellData.Coords, 0, true, false, clearSelection);
               // cellSelector.MarkRange(cellData.Coords, 1, true, false, clearSelection);
            }
            else
                cellSelector.ClearSelection();

            yield return null;
        }
    }

    #endregion
}
