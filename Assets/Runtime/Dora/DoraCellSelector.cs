using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoraCellSelector : MonoBehaviourBase, IRangeSelectionProvider
{
    [SerializeField] int maxSelectionRadius = 3;

    DoraCellMap cellMap = null;
    Dictionary<Vector2Int, DoraCellData> selectedRange = null;    
    List<HashSet<Vector2Int>> selectedRangeInSteps = null;    
    List<Vector2Int> recursiveSelectBuffer = null;
    List<Vector2Int> lastRecursiveSelect = null;
    Vector2Int? currentOriginCell = null;
    Vector2Int? previousOriginCell = null;
    Transform currentRowNormal = null;
    int currentRowIndex = 0;

    #region IRangeSelectionProvider

    public int MaxSelectionRadius => maxSelectionRadius;

    public Vector2Int? PreviousOriginCell => previousOriginCell;

    public Vector2Int? CurrentOriginCell => currentOriginCell;

    public IReadOnlyList<Vector2Int> SelectedRange => null == selectedRange ? null : selectedRange.Keys.ToList();

    public IReadOnlyList<HashSet<Vector2Int>> SelectedRangeInSteps => selectedRangeInSteps;

    #endregion

    #region MUTABLE

    public void SetCellMap(DoraCellMap i_cellMap)
    {
        ClearSelection();
        cellMap = i_cellMap;
    }

    [ExposePublicMethod]
    public void SelectCell(Vector2Int i_cell, bool i_loopCoords, bool i_clearSelection)
    {
        if (true == i_clearSelection)
            ClearSelection();

        processCell(i_cell, 0, i_loopCoords, i_loopCoords, true, true);
    }

    [ExposePublicMethod]
    public void SelectRange(Vector2Int i_origin, int i_radius, bool i_loopX, bool i_loopY, bool i_clearSelection)
    {
        if (true == i_clearSelection)
            ClearSelection();

        i_radius = Mathf.Clamp(i_radius, 0, maxSelectionRadius);

        processCell(i_origin, 0, i_loopX, i_loopY, true, true);

        if (null == lastRecursiveSelect) lastRecursiveSelect = new List<Vector2Int>();
        lastRecursiveSelect.Clear();

        lastRecursiveSelect.Add(i_origin);
        int currRadius = 0;
        int currStep = 1;
        selectRecursive(ref lastRecursiveSelect, ref currRadius, ref currStep, i_radius, i_loopX, i_loopY);
    } 

    public void SelectAll()
    {
        DoraCellData[] allCells = cellMap.AllCells;
        int count = allCells.Length;

        for(int i = 0; i < count; i++)
        {
            processCell(allCells[i].Coords, 1, false, false, i == 0, false);
        }
    }

    [ExposePublicMethod]
    public void ClearSelection()
    {
        previousOriginCell = currentOriginCell;

        currentRowNormal = null;
        currentOriginCell = null;

        if (null == selectedRange) return;

        foreach (KeyValuePair<Vector2Int, DoraCellData> pair in selectedRange)
            pair.Value.Unselect(true);

        selectedRange.Clear();

        if (selectedRangeInSteps == null) return;
        foreach (HashSet<Vector2Int> selectedStep in selectedRangeInSteps)
            selectedStep.Clear();
    }

    #endregion

    #region PRIVATE
    bool processCell(Vector2Int i_coord, int i_stepIdx, bool i_loopX, bool i_loopY, bool i_isOriginCell, bool i_updateNormal)
    {
        if (null == cellMap) return false;

        if (null == selectedRange) selectedRange = new Dictionary<Vector2Int, DoraCellData>(1 + maxSelectionRadius * maxSelectionRadius * 4);

        if (selectedRangeInSteps == null)
        {
            selectedRangeInSteps = new List<HashSet<Vector2Int>>(maxSelectionRadius + 1);
            int length = maxSelectionRadius + 1;
            for (int i = 0; i < length; i++)
                selectedRangeInSteps.Add(new HashSet<Vector2Int>());
        }

        i_coord = cellMap.GetLoopedCoord(i_coord, i_loopX, i_loopY);

        if (false == selectedRange.ContainsKey(i_coord))
        {
            DoraCellData cell = cellMap.GetCell(i_coord, false, false);

            if (null == cell) return false;

            selectedRange.Add(i_coord, cell);

            if (i_stepIdx >= 0 && i_stepIdx < selectedRangeInSteps.Count)
                selectedRangeInSteps[i_stepIdx].Add(i_coord);

            cell.Select(true);

            if (true == i_updateNormal)
                updateRowIndex(cell.Coords.x);

            if (true == i_isOriginCell)
                currentOriginCell = cell.Coords;

            return true;
        }

        return false;
    }

    void addToSelectionBuffer(Vector2Int i_coord, int i_currStep, int i_offsetX, int i_offsetY, bool i_loopX, bool i_loopY)
    {
        if (null == recursiveSelectBuffer) recursiveSelectBuffer = new List<Vector2Int>();

        Vector2Int selectedCoord = i_coord;

        selectedCoord.x = i_coord.x + i_offsetX;
        selectedCoord.y = i_coord.y + i_offsetY;

        processCell(selectedCoord, i_currStep, i_loopX, i_loopY, false, false);
        recursiveSelectBuffer.Add(selectedCoord);
    }

    void selectRecursive(ref List<Vector2Int> i_lastRecursiveSelect, ref int i_currRadius, ref int i_currStep, int i_radius, bool i_loopX, bool i_loopY)
    {
        //Debug.Log(i_currRadius + "  " + i_radius);
        if (i_currRadius == i_radius) return;
        if (null == recursiveSelectBuffer) recursiveSelectBuffer = new List<Vector2Int>();

        int count = i_lastRecursiveSelect.Count;
        for (int i = 0; i < count; i++)
        {
            Vector2Int currCoord = i_lastRecursiveSelect[i];
            addToSelectionBuffer(currCoord, i_currStep, 0, -1, i_loopX, i_loopY);
            addToSelectionBuffer(currCoord, i_currStep, 1, 0, i_loopX, i_loopY);
            addToSelectionBuffer(currCoord, i_currStep, 0, 1, i_loopX, i_loopY);
            addToSelectionBuffer(currCoord, i_currStep, -1, 0, i_loopX, i_loopY);
        }

        i_currRadius++;
        i_lastRecursiveSelect.Clear();
        i_lastRecursiveSelect.AddRange(recursiveSelectBuffer);
        recursiveSelectBuffer.Clear();

        i_currStep++;
        selectRecursive(ref i_lastRecursiveSelect, ref i_currRadius, ref i_currStep, i_radius, i_loopX, i_loopY);
    }

    void updateRowIndex(int i_rowIndex)
    {
        currentRowIndex = i_rowIndex;
        currentRowNormal = cellMap.GetRowNormal(currentRowIndex, true);
    }

    #endregion
}

