using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class DoraCellSelector : MonoBehaviourBase
{
    [SerializeField] DoraCellMap cellMap = null;
    [SerializeField] int maxSelectionRadius = 3;

    Dictionary<Vector2Int, DoraCellData> selectedRange = null;
    List<Vector2Int> selectionBuffer = null;

    #region PUBLIC API

    [ExposePublicMethod]
    public void SelectCell(Vector2Int i_cell, bool i_loopCoords)
    {
        trySelectCell(i_cell, i_loopCoords, i_loopCoords);
    }

    [ExposePublicMethod]
    public void SelectRange(Vector2Int i_origin, int i_radius, bool i_loopX, bool i_loopY)
    {
        i_radius = Mathf.Clamp(i_radius, 0, maxSelectionRadius);

        trySelectCell(i_origin, i_loopX, i_loopY);

        List<Vector2Int> lastSelected = new List<Vector2Int>();
        lastSelected.Add(i_origin);
        int currRadius = 0;
        selectRecursive(ref lastSelected, ref currRadius, i_radius, i_loopX, i_loopY);
    }

    #endregion

    [ExposePublicMethod]
    public void ClearSelection()
    {
        if (null == selectedRange) return;

        foreach (KeyValuePair<Vector2Int, DoraCellData> pair in selectedRange)
        {
            pair.Value.Unselect();
        }

        selectedRange.Clear();
    }

    #region PRIVATE

    bool trySelectCell(Vector2Int i_coord, bool i_loopX, bool i_loopY)
    {
        if (null == selectedRange) selectedRange = new Dictionary<Vector2Int, DoraCellData>(maxSelectionRadius * maxSelectionRadius * 4);

        if (false == selectedRange.ContainsKey(i_coord))
        {
            DoraCellData cell = cellMap.GetCell(i_coord, i_loopX, i_loopY);
            selectedRange.Add(i_coord, cell);
            cell.Select();
            return true;
        }

        return false;
    }

    void addToSelectionBuffer(Vector2Int i_coord, int i_offsetX, int i_offsetY, bool i_loopX, bool i_loopY)
    {
        if (null == selectionBuffer) selectionBuffer = new List<Vector2Int>();

        Vector2Int selectedCoord = i_coord;

        selectedCoord.x = i_coord.x + i_offsetX;
        selectedCoord.y = i_coord.y + i_offsetY;

        trySelectCell(selectedCoord, i_loopX, i_loopY);
        selectionBuffer.Add(selectedCoord);
    }

    void selectRecursive(ref List<Vector2Int> i_lastSelected, ref int i_currRadius, int i_radius, bool i_loopX, bool i_loopY)
    {
        Debug.Log(i_currRadius + "  " + i_radius);
        if (i_currRadius == i_radius) return;
        if (null == selectionBuffer) selectionBuffer = new List<Vector2Int>();

        int count = i_lastSelected.Count;
        for (int i = 0; i < count; i++)
        {
            Vector2Int currCoord = i_lastSelected[i];
            addToSelectionBuffer(currCoord, 0, -1, i_loopX, i_loopY);
            addToSelectionBuffer(currCoord, 1, 0, i_loopX, i_loopY);
            addToSelectionBuffer(currCoord, 0, 1, i_loopX, i_loopY);
            addToSelectionBuffer(currCoord, -1, 0, i_loopX, i_loopY);
        }

        i_currRadius++;
        i_lastSelected.Clear();
        i_lastSelected.AddRange(selectionBuffer);
        selectionBuffer.Clear();

        selectRecursive(ref i_lastSelected, ref i_currRadius, i_radius, i_loopX, i_loopY);
    }

    #endregion
}
