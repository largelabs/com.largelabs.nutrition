using System.Collections.Generic;
using UnityEngine;


public class DoraCellSelector : MonoBehaviourBase
{
    [SerializeField] DoraCellMap cellMap = null;
    [SerializeField] int maxSelectionRadius = 3;
    [SerializeField] Vector2Int defaultSelect = new Vector2Int(5, 5);
    [SerializeField] [Range(100f, 500f)] float rotationSpeed = 200f;

    Dictionary<Vector2Int, DoraCellData> selectedRange = null;
    List<Vector2Int> recursiveSelectBuffer = null;
    List<Vector2Int> lastRecursiveSelect = null;
    Transform rowNormal = null;

    int currentRowIndex = 0;

    #region UNITY AND CORE

    private void Start()
    {
        SelectCell(defaultSelect, true, true);
    }

    private void Update()
    {
        float dot = Vector3.Dot(rowNormal.forward, Camera.main.transform.forward);

        if (dot < -1.1f || dot > -0.97f)
        {
            int sign = dot < -1f ? -1 : 1;
            cellMap.transform.Rotate(Time.deltaTime * sign * rotationSpeed, 0f, 0f);
        }
    }

    #endregion

    #region PUBLIC API

    [ExposePublicMethod]
    public void SelectCell(Vector2Int i_cell, bool i_loopCoords, bool i_clearSelection)
    {
        if (true == i_clearSelection) ClearSelection();

        trySelectCell(i_cell, i_loopCoords, i_loopCoords, true);
    }

    [ExposePublicMethod]
    public void SelectRange(Vector2Int i_origin, int i_radius, bool i_loopX, bool i_loopY, bool i_clearSelection)
    {
        if (true == i_clearSelection) ClearSelection();

        i_radius = Mathf.Clamp(i_radius, 0, maxSelectionRadius);

        trySelectCell(i_origin, i_loopX, i_loopY, true);

        if (null == lastRecursiveSelect) lastRecursiveSelect = new List<Vector2Int>();
        lastRecursiveSelect.Clear();

        lastRecursiveSelect.Add(i_origin);
        int currRadius = 0;
        selectRecursive(ref lastRecursiveSelect, ref currRadius, i_radius, i_loopX, i_loopY);
    }

    #endregion

    [ExposePublicMethod]
    public void ClearSelection()
    {
        if (null == selectedRange) return;

        foreach (KeyValuePair<Vector2Int, DoraCellData> pair in selectedRange)
            pair.Value.Unselect();

        selectedRange.Clear();
    }

    #region PRIVATE

    bool trySelectCell(Vector2Int i_coord, bool i_loopX, bool i_loopY, bool i_updateNormal)
    {
        if (null == selectedRange) selectedRange = new Dictionary<Vector2Int, DoraCellData>(1 + maxSelectionRadius * maxSelectionRadius * 4);

        if (false == selectedRange.ContainsKey(i_coord))
        {
            DoraCellData cell = cellMap.GetCell(i_coord, i_loopX, i_loopY);
            selectedRange.Add(i_coord, cell);
            cell.Select();

            if(true == i_updateNormal)
            {
                updateRowIndex(cell.Coords.x);
            }

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

        trySelectCell(selectedCoord, i_loopX, i_loopY, false);
        recursiveSelectBuffer.Add(selectedCoord);
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

    void updateRowIndex(int i_rowIndex)
    {
        currentRowIndex = i_rowIndex;
        rowNormal = cellMap.GetRowNormal(currentRowIndex, true);
    }

    #endregion
}
