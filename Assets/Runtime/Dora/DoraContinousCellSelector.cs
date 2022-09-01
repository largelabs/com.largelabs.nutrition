
using System.Collections;
using UnityEngine;

public class DoraContinousCellSelector : DoraAbstractCellSelector, IRangeSelectionProvider
{    
    [SerializeField] float rotationSpeed = 50f;

    protected override IEnumerator updateRotation(Transform i_nextRowNormal, int i_nextRowIndex)
    {
        float dot = 0f;
        rotateCob(i_nextRowNormal, Time.deltaTime * rotationSpeed, out dot);
        if (true == isDotProductAligned(dot))
        {
            Vector2Int nextCell = CurrentOriginCell.Value;
            nextCell.x = i_nextRowIndex;
            SelectCell(nextCell, true, true);
        }

        yield return null;
    }
}
