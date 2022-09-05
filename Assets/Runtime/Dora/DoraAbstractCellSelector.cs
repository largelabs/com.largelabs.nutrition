
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IRangeSelectionProvider
{
    int MaxSelectionRadius { get; }

    Vector2Int? CurrentOriginCell { get; }

    IReadOnlyList<Vector2Int> SelectedRange { get; }
}

public abstract class DoraAbstractCellSelector : MonoBehaviourBase, IRangeSelectionProvider
{
    [SerializeField] bool autoUpdateYSelection = true;
    [SerializeField] int maxSelectionRadius = 3;
    [SerializeField] Vector2Int defaultSelect = new Vector2Int(5, 5);

    DoraCellMap cellMap = null;
    Dictionary<Vector2Int, DoraCellData> selectedRange = null;
    Dictionary<Vector2Int, DoraCellData> markedRange = null;
    List<Vector2Int> recursiveSelectBuffer = null;
    List<Vector2Int> lastRecursiveSelect = null;

    List<Vector2Int> recursiveMarkingBuffer = null;
    List<Vector2Int> lastRecursiveMark = null;

    Vector2Int? currentOriginCell = null;
    Vector2Int? previousOriginCell = null;
    Transform currentRowNormal = null;
    Transform nextRowNormal = null;
    Coroutine autoRotationRoutine = null;

    int currentRowIndex = 0;
    int nextRowIndex = 0;

    #region IRangeSelectionProvider

    public int MaxSelectionRadius => maxSelectionRadius;

    public Vector2Int? PreviousOriginCell => previousOriginCell;

    public Vector2Int? CurrentOriginCell => currentOriginCell;

    public IReadOnlyList<Vector2Int> SelectedRange => null == selectedRange ? null : selectedRange.Keys.ToList();

    #endregion

    #region MUTABLE

    public void StartAutoRotation()
    {
        if (null != autoRotationRoutine) return;
        autoRotationRoutine = StartCoroutine(autoRotationLoop());
    }

    public void StopAutoRotation()
    {
        if (null == autoRotationRoutine) return;
        this.DisposeCoroutine(ref autoRotationRoutine);
    }

    public void SetCellMap(DoraCellMap i_cellMap)
    {
        ClearSelection();

        cellMap = i_cellMap;
        SelectCell(defaultSelect, true, true);
    }

    [ExposePublicMethod]
    public void SelectCell(Vector2Int i_cell, bool i_loopCoords, bool i_clearSelection)
    {
        if (true == i_clearSelection) ClearSelection();

        processCell(i_cell, i_loopCoords, i_loopCoords, true, true, false);
    }

    [ExposePublicMethod]
    public void SelectRange(Vector2Int i_origin, int i_radius, bool i_loopX, bool i_loopY, bool i_clearSelection)
    {
        if (true == i_clearSelection) ClearSelection();

        i_radius = Mathf.Clamp(i_radius, 0, maxSelectionRadius);

        processCell(i_origin, i_loopX, i_loopY, true, true, false);

        if (null == lastRecursiveSelect) lastRecursiveSelect = new List<Vector2Int>();
        lastRecursiveSelect.Clear();

        lastRecursiveSelect.Add(i_origin);
        int currRadius = 0;
        selectRecursive(ref lastRecursiveSelect, ref currRadius, i_radius, i_loopX, i_loopY);
    }

    public void MarkRange(Vector2Int i_origin, int i_radius, bool i_loopX, bool i_loopY, bool i_clearMarking)
    {
        if (true == i_clearMarking) ClearMarking();

        i_radius = Mathf.Clamp(i_radius, 0, maxSelectionRadius);

        processCell(i_origin, i_loopX, i_loopY, true, true, false);

        if (null == lastRecursiveMark) lastRecursiveMark = new List<Vector2Int>();
        lastRecursiveMark.Clear();

        lastRecursiveMark.Add(i_origin);
        int currRadius = 0;
        markRecursive(ref lastRecursiveMark, ref currRadius, i_radius, i_loopX, i_loopY);
    }

    [ExposePublicMethod]
    public void ClearSelection()
    {
        ClearMarking();

        previousOriginCell = currentOriginCell;

        currentRowNormal = null;
        currentOriginCell = null;

        if (null == selectedRange) return;

        foreach (KeyValuePair<Vector2Int, DoraCellData> pair in selectedRange)
            pair.Value.Unselect(true);

        selectedRange.Clear();
    }

    [ExposePublicMethod]
    public void ClearMarking()
    {
        if (null == markedRange) return;

        foreach (KeyValuePair<Vector2Int, DoraCellData> pair in markedRange)
            pair.Value.UnmarkForSelection(true);

        markedRange.Clear();
    }

    #endregion

    #region PROTECTED API

    protected abstract IEnumerator updateRotation(Transform i_nextRowNormal, int i_nextRowIndex, bool i_autoUpdateSelection);

    protected void rotateCob(Transform i_rowNormal, float i_amount, out float i_dot)
    {
        cellMap.transform.Rotate(i_amount, 0f, 0f);
        i_dot = Vector3.Dot(i_rowNormal.forward, Camera.main.transform.forward);
    }

    protected bool isDotProductAligned(float i_dot)
    {
        return i_dot > -1.02f && i_dot < -0.98f;
    }

    #endregion

    #region PRIVATE

    bool processCell(Vector2Int i_coord, bool i_loopX, bool i_loopY, bool i_isOriginCell, bool i_updateNormal, bool i_markOnly)
    {
        if (null == cellMap) return false;

        Dictionary<Vector2Int, DoraCellData> currDic = null;

        if(false == i_markOnly)
        {
            if (null == selectedRange) selectedRange = new Dictionary<Vector2Int, DoraCellData>(1 + maxSelectionRadius * maxSelectionRadius * 4);
            currDic = selectedRange;
        }
        else
        {
            if (null == markedRange) markedRange = new Dictionary<Vector2Int, DoraCellData>(1 + maxSelectionRadius * maxSelectionRadius * 4);
            currDic = markedRange;
        }

        i_coord = cellMap.GetLoopedCoord(i_coord, i_loopX, i_loopY);

        if (false == currDic.ContainsKey(i_coord))
        {
            DoraCellData cell = cellMap.GetCell(i_coord, false, false);

            if (null == cell) return false;

            currDic.Add(i_coord, cell);

            if (false == i_markOnly)
                cell.Select(true);
            else
                cell.MarkForSelection(true);

            if (true == i_updateNormal)
                updateRowIndex(cell.Coords.x);

            if (true == i_isOriginCell)
                currentOriginCell = cell.Coords;

            return true;
        }

        return false;
    }

    void addToSelectionBuffer(Vector2Int i_coord, int i_offsetX, int i_offsetY, bool i_loopX, bool i_loopY)
    {
        if (null == recursiveSelectBuffer) recursiveSelectBuffer = new List<Vector2Int>();

        Vector2Int selectedCoord = i_coord;

        selectedCoord.x = i_coord.x + i_offsetX;
        selectedCoord.y = i_coord.y + i_offsetY;

        processCell(selectedCoord, i_loopX, i_loopY, false, false, false);
        recursiveSelectBuffer.Add(selectedCoord);
    }

    void addToMarkBuffer(Vector2Int i_coord, int i_offsetX, int i_offsetY, bool i_loopX, bool i_loopY)
    {
        if (null == recursiveMarkingBuffer) recursiveMarkingBuffer = new List<Vector2Int>();

        Vector2Int selectedCoord = i_coord;

        selectedCoord.x = i_coord.x + i_offsetX;
        selectedCoord.y = i_coord.y + i_offsetY;

        processCell(selectedCoord, i_loopX, i_loopY, false, false, true);
        recursiveMarkingBuffer.Add(selectedCoord);
    }

    void selectRecursive(ref List<Vector2Int> i_lastRecursiveSelect, ref int i_currRadius, int i_radius, bool i_loopX, bool i_loopY)
    {
        Debug.Log(i_currRadius + "  " + i_radius);
        if (i_currRadius == i_radius) return;
        if (null == recursiveSelectBuffer) recursiveSelectBuffer = new List<Vector2Int>();

        int count = i_lastRecursiveSelect.Count;
        for (int i = 0; i < count; i++)
        {
            Vector2Int currCoord = i_lastRecursiveSelect[i];
            addToSelectionBuffer(currCoord, 0, -1, i_loopX, i_loopY);
            addToSelectionBuffer(currCoord, 1, 0, i_loopX, i_loopY);
            addToSelectionBuffer(currCoord, 0, 1, i_loopX, i_loopY);
            addToSelectionBuffer(currCoord, -1, 0, i_loopX, i_loopY);
        }

        i_currRadius++;
        i_lastRecursiveSelect.Clear();
        i_lastRecursiveSelect.AddRange(recursiveSelectBuffer);
        recursiveSelectBuffer.Clear();

        selectRecursive(ref i_lastRecursiveSelect, ref i_currRadius, i_radius, i_loopX, i_loopY);
    }

    void markRecursive(ref List<Vector2Int> i_lastRecursiveMark, ref int i_currRadius, int i_radius, bool i_loopX, bool i_loopY)
    {
        Debug.Log(i_currRadius + "  " + i_radius);
        if (i_currRadius == i_radius) return;
        if (null == recursiveMarkingBuffer) recursiveMarkingBuffer = new List<Vector2Int>();

        int count = i_lastRecursiveMark.Count;
        for (int i = 0; i < count; i++)
        {
            Vector2Int currCoord = i_lastRecursiveMark[i];
            addToMarkBuffer(currCoord, 0, -1, i_loopX, i_loopY);
            addToMarkBuffer(currCoord, 1, 0, i_loopX, i_loopY);
            addToMarkBuffer(currCoord, 0, 1, i_loopX, i_loopY);
            addToMarkBuffer(currCoord, -1, 0, i_loopX, i_loopY);
        }

        i_currRadius++;
        i_lastRecursiveMark.Clear();
        i_lastRecursiveMark.AddRange(recursiveMarkingBuffer);
        recursiveMarkingBuffer.Clear();

        markRecursive(ref i_lastRecursiveMark, ref i_currRadius, i_radius, i_loopX, i_loopY);
    }

    void updateRowIndex(int i_rowIndex)
    {
        currentRowIndex = i_rowIndex;
        Vector2Int upperCell = cellMap.GetLoopedCoord(new Vector2Int(currentRowIndex + 1, 0), true, false);
        nextRowIndex = upperCell.x;
        currentRowNormal = cellMap.GetRowNormal(currentRowIndex, true);
        nextRowNormal = cellMap.GetRowNormal(nextRowIndex, true);
    }

    IEnumerator autoRotationLoop()
    {
        while (true)
        {
            yield return StartCoroutine(updateRotation(nextRowNormal, nextRowIndex, autoUpdateYSelection));
        }
    }

    #endregion

}

