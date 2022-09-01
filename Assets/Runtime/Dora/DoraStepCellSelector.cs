﻿
using System.Collections;
using UnityEngine;

public class DoraStepCellSelector : DoraAbstractCellSelector, IRangeSelectionProvider
{
    [SerializeField] float rotationSpeed = 50f;
    [SerializeField] float rowTime = 2f;

    Coroutine rotateToNextStepRoutine = null;

    protected override IEnumerator updateRotation(Transform i_nextRowNormal, int i_nextRowIndex)
    {
        rotateToNextStepRoutine = StartCoroutine(rotateToNextStep(i_nextRowNormal, i_nextRowIndex));
        while (null != rotateToNextStepRoutine) yield return null;
        yield return this.Wait(rowTime);
    }

    IEnumerator rotateToNextStep(Transform i_nextRowNormal, int i_nextRowIndex)
    {
        while (true)
        {
            float dot = 0f;
            rotateCob(i_nextRowNormal, Time.deltaTime * rotationSpeed, out dot);
            if (true == isDotProductAligned(dot))
            {
                Vector2Int nextCell = CurrentOriginCell.Value;
                nextCell.x = i_nextRowIndex;
                SelectCell(nextCell, true, true);

                this.DisposeCoroutine(ref rotateToNextStepRoutine);
                yield break;
            }
            else
                yield return null;
        }
    }
}
