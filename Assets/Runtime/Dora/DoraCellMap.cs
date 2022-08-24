using System.Collections;
using UnityEngine;

public class DoraCellMap : MonoBehaviourBase
{
    [SerializeField] Transform[] anchors = null;
    [SerializeField] Transform[] normalAnchors = null;
    [SerializeField] GameObject kernel = null;
    [SerializeField] InterpolatorsManager interpolators = null;

    DoraCellData[,] cellMap = null;
    int currentRowIndex = 0;
    int currentColumnIndex = 0;
    Transform rowNormal = null;
    DoraCellFactory cellFactory = null;

    private const int NB_ROWS = 12;
    private const int NB_COLUMNS = 11;

    #region UNITY AND CORE

    private void Start()
    {
        PopulateMap();
        updateRowIndex(0);

        RevealCells(true);
    }

    private void Update()
    {
        float dot = Vector3.Dot(rowNormal.forward, Camera.main.transform.forward);

        if(dot < -1.1f || dot > -0.97f)
        {
            int sign = dot < -1f ? -1 : 1;
            transform.Rotate(Time.deltaTime * sign * 200f, 0f, 0f);
        }
    }

    #endregion

    #region PUBLIC API

    public void RevealCells(bool i_animated)
    {
        if (null == cellMap) return;

        if(true == i_animated)
        {
            StartCoroutine(revealCellsAnimated(0, NB_ROWS / 2, true));
            StartCoroutine(revealCellsAnimated(NB_ROWS / 2, NB_ROWS, false));
        }
        else
        {
            for (int i = 0; i < NB_ROWS; i++)
            {
                for (int j = 0; j < NB_COLUMNS; j++)
                {
                    if (true == cellMap[i, j].HasKernel)
                    {
                        cellMap[i, j].Kernel.Appear(false);
                    }
                }
            }
            updateRowIndex(0);
        }
    }

    public void PopulateMap()
    {
        int count = anchors.Length;

        DoraCellData[] cellBuffer = new DoraCellData[count];
        DoraCellData currData = null;

        if (null == cellFactory) cellFactory = new DoraCellFactory(interpolators);

        for (int i = 0; i < count; i++)
        {
            cellBuffer[i] = cellFactory.MakeCell(anchors[i], kernel, i);

        }

        cellMap = CollectionUtilities.Make2DArray<DoraCellData>(cellBuffer, NB_ROWS, NB_COLUMNS);

        for(int i = 0; i < 12; i++)
        {
            for(int j = 0; j < 11; j++)
            {
                currData = cellMap[i, j];
                currData.SetCoords(new Vector2Int(i, j));
                currData.SetKernelName(i + "," + j);
            }
        }
    }

    public DoraCellData GetCell(Vector2Int i_coord, bool i_loopX, bool i_loopY)
    {
        if (null == cellMap) return null;
        Vector2Int fixedCoord = getCoords(i_coord, i_loopX, i_loopY);
        return cellMap[fixedCoord.x, fixedCoord.y];
    }

    #endregion

    #region PRIVATE


    IEnumerator revealCellsAnimated(int i_startRow, int i_endRow, bool i_updateRowIndex)
    {
        for (int i = i_startRow; i < i_endRow; i++)
        {
            if(true == i_updateRowIndex) updateRowIndex(i);
            for (int j = 0; j < 11; j++)
            {
                if(true == cellMap[i, j].HasKernel)
                {
                    cellMap[i, j].Kernel.Appear(true);
                    yield return this.Wait(0.025f);
                }
            }
        }
    }


    void updateRowIndex(int i_rowIndex)
    {
        currentRowIndex = i_rowIndex;
        rowNormal = normalAnchors[currentRowIndex];
    }


    int getLoopedInteger(int i_input, int i_maxValue)
    {
        if (i_input >= 0 && i_input < i_maxValue)
            return i_input;

        int ret = 0;

        if (i_input < 0)
        {
            int moduloOffset = i_input % i_maxValue;
            ret = moduloOffset == 0 ? 0 : i_input % i_maxValue + i_maxValue;
        }
        else
            ret = Mathf.Abs(i_input - i_maxValue) % i_maxValue;

        return ret;
    }

    Vector2Int getCoords(Vector2Int i_coord, bool i_loopX, bool i_loopY)
    {
        i_coord.x = i_loopX ? getLoopedInteger(i_coord.x, NB_ROWS) : Mathf.Clamp(i_coord.x, 0, NB_ROWS - 1);
        i_coord.y = i_loopY ? getLoopedInteger(i_coord.y, NB_COLUMNS) : Mathf.Clamp(i_coord.y, 0, NB_COLUMNS - 1);
        return i_coord;
    }

    #endregion

}
