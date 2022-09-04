
using System.Collections;
using UnityEngine;

public class DoraContinousCellSelector : DoraAbstractCellSelector, IRangeSelectionProvider
{    
    [SerializeField] float rotationSpeed = 50f;

    protected override IEnumerator updateRotation(Transform i_nextRowNormal, int i_nextRowIndex, bool i_autoUpdateSelection)
    {
        float dot = 0f;
        rotateCob(i_nextRowNormal, Time.deltaTime * rotationSpeed, out dot);

        if (true == isDotProductAligned(dot))
        {
            if (true == i_autoUpdateSelection && null != CurrentOriginCell)
            {
                Vector2Int nextCell = CurrentOriginCell.Value;
                nextCell.x = i_nextRowIndex;
                SelectCell(nextCell, true, true);
            }
        }



        yield return null;
    }
}
