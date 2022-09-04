
using System.Collections;
using UnityEngine;

public class DoraStepCellSelector : DoraAbstractCellSelector, IRangeSelectionProvider
{
    [SerializeField] float rotationSpeed = 50f;
    [SerializeField] float rowTime = 2f;

    Coroutine rotateToNextStepRoutine = null;

    protected override IEnumerator updateRotation(Transform i_nextRowNormal, int i_nextRowIndex, bool i_autoUpdateSelection)
    {
        rotateToNextStepRoutine = StartCoroutine(rotateToNextStep(i_nextRowNormal, i_nextRowIndex, i_autoUpdateSelection));
        while (null != rotateToNextStepRoutine) yield return null;
        yield return this.Wait(rowTime);
    }

    IEnumerator rotateToNextStep(Transform i_nextRowNormal, int i_nextRowIndex, bool i_autoUpdateSelection)
    {
        while (true)
        {
            float dot = 0f;
            rotateCob(i_nextRowNormal, Time.deltaTime * rotationSpeed, out dot);
            if (true == isDotProductAligned(dot))
            {
                if(true == i_autoUpdateSelection && null != CurrentOriginCell)
                {
                    Vector2Int nextCell = CurrentOriginCell.Value;
                    nextCell.x = i_nextRowIndex;
                    SelectCell(nextCell, true, true);
                }

                this.DisposeCoroutine(ref rotateToNextStepRoutine);
                yield break;
            }
            else
                yield return null;
        }
    }
}
