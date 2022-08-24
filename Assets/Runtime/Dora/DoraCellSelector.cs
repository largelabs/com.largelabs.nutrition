using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class DoraCellSelector : MonoBehaviourBase
{
    [SerializeField] DoraCellMap cellMap = null;
    [SerializeField] int maxSelectionRange = 3;

    List<DoraCellData> selectedRange = null;

    [ExposePublicMethod]
    public void SelectRange(Vector2Int i_origin, int i_rangeX, int i_rangeY, bool i_loopX, bool i_loopY)
    {
        i_rangeX = Mathf.Clamp(i_rangeX, 0, 5);
        i_rangeY = Mathf.Clamp(i_rangeY, 0, 5);

        if (null == selectedRange) selectedRange = new List<DoraCellData>(maxSelectionRange * maxSelectionRange);
        selectedRange.Clear();

        DoraCellData currCell = null;
        Vector2Int currCoord = i_origin;

        for (int i = 0; i < i_rangeX; i++)
        {
            currCoord.x = i_origin.x + i;
            for (int j = 0; j < i_rangeY; j++)
            {
                currCoord.y = i_origin.y + j;
                currCell = cellMap.GetCell(currCoord, i_loopX, i_loopY);
                selectedRange.Add(currCell);

                currCell.Select();
            }
        }

        int count = selectedRange.Count;
        Debug.Log("Did select " + count + " cells");

        for (int i = 0; i < count; i++)
        {
            Debug.Log(selectedRange[i].Coords);
        }
    }

    [ExposePublicMethod]
    public void ClearSelection()
    {
        if (null == selectedRange) return;

        int count = selectedRange.Count;
        for(int i = 0; i < count; i++)
        {
            selectedRange[i].Unselect();
        }

        selectedRange.Clear();
    }
}
