using System.Collections;
using UnityEngine;

public class DoraCellMap : MonoBehaviourBase
{
    [SerializeField] Transform[] anchors = null;
    [SerializeField] Transform[] normalAnchors = null;
    [SerializeField] KernelSpawner kernelSpawner = null;
    //[SerializeField] GameObject kernel = null;
    [SerializeField] InterpolatorsManager interpolators = null;
    [SerializeField] DoraData doraData = null;

    DoraCellData[,] cellMap = null;
    DoraCellFactory cellFactory = null;

    public DoraData DoraData => doraData;
    public int cellMapLength0 => cellMap.GetLength(0);
    public int cellMapLength1 => cellMap.GetLength(1);

    private const int NB_ROWS = 12;
    private const int NB_COLUMNS = 11;

    #region UNITY AND CORE

    protected override void Awake()
    {
        base.Awake();
        PopulateMap();
        RevealCells(true);
    }

    #endregion

    #region PUBLIC API

    public void RevealCells(bool i_animated)
    {
        if (null == cellMap) return;

        if(true == i_animated)
        {
            StartCoroutine(revealCellsAnimated(0, NB_ROWS / 2));
            StartCoroutine(revealCellsAnimated(NB_ROWS / 2, NB_ROWS));
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
        }
    }

    public void PopulateMap()
    {
        if (kernelSpawner == null)
        {
            Debug.LogError("Kernel spawner is null...Cannot populate map!");
            return;
        }


        int count = anchors.Length;

        DoraCellData[] cellBuffer = new DoraCellData[count];
        DoraCellData currData = null;

        if (null == cellFactory) cellFactory = new DoraCellFactory(interpolators);

        DoraKernel currentKernel = null;
        for (int i = 0; i < count; i++)
        {
            currentKernel = kernelSpawner.SpawnDoraKernelWithAnchor(anchors[i]);
            if (currentKernel != null)
                cellBuffer[i] = cellFactory.MakeCell(currentKernel);
            else
            {
                Debug.LogError("Factory could not create cell! Returning...");
                return;
            }
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

    public Vector2Int GetLoopedCoord(Vector2Int i_coord, bool i_loopX, bool i_loopY)
    {
        return getCoords(i_coord, i_loopX, i_loopY);
    }

    public Transform GetRowNormal(int i_index, bool i_loop)
    {
        i_index = i_loop ? getLoopedInteger(i_index, NB_ROWS) : Mathf.Clamp(i_index, 0, NB_ROWS - 1);
        return normalAnchors[i_index];
    }

    #endregion

    #region PRIVATE

    IEnumerator revealCellsAnimated(int i_startRow, int i_endRow)
    {
        for (int i = i_startRow; i < i_endRow; i++)
        {
            for (int j = 0; j < NB_COLUMNS; j++)
            {
                if (true == cellMap[i, j].HasKernel)
                {
                    cellMap[i, j].Kernel.Appear(true);
                    yield return this.Wait(0.025f);
                }
            }
        }
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
