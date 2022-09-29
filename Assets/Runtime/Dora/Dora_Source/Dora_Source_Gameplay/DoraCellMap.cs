using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoraCellMap : MonoBehaviourBase, IDoraCellProvider
{
    [SerializeField] Transform[] anchors = null;
    [SerializeField] Transform[] normalAnchors = null;
    [SerializeField] KernelSpawner kernelSpawner = null;
    [SerializeField] DoraDurabilityManager durabilityManager = null;
    [SerializeField] InterpolatorsManager interpolators = null;
    [SerializeField] MeshRenderer cobRnd = null;

    // Collections
    DoraCellData[] cells = null;
    DoraCellData[,] cellMap = null;
    Dictionary<GameObject, DoraCellData> cellsByGo = null;

    private DoraData doraData = null;

    BoxCollider cullingBounds = null;
    BoxCollider selectionBounds = null;

    private const int NB_ROWS = 12;
    private const int NB_COLUMNS = 11;

    private void Update()
    {
        foreach (DoraCellData doraCell in cellMap)
        {
            if (true == doraCell.HasKernel)
            {
                doraCell.EnableKernelRenderer(cullingBounds.bounds.Intersects(doraCell.GetCellBounds()));
                doraCell.EnableKernelCollider(selectionBounds.bounds.Intersects(doraCell.GetCellBounds()));
            }
        }
    }

    #region IDoraCellProvider

    public DoraCellData[] AllCells => cells;

    public int CellMapLength0 => cellMap.GetLength(0);
    public int CellMapLength1 => cellMap.GetLength(1);

    public DoraCellData GetCell(GameObject i_go)
    {
        if (null == i_go) return null;
        if (null == cellsByGo) return null;
        DoraCellData ret = null;
        cellsByGo.TryGetValue(i_go, out ret);

        return ret;
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

    public int GetKernelCount()
    {
        if (cellMap == null) return 0;

        int ret = 0;
        int length0 = CellMapLength0;
        int length1 = CellMapLength1;

        for (int i = 0; i < length0; i++)
        {
            for (int j = 0; j < length1; j++)
            {
                if (cellMap[i, j].HasKernel) ret++;
            }
        }

        return ret;
    }

    public int TotalCellCount => CellMapLength0 * CellMapLength1;

    #endregion

    #region PUBLIC API
    public float BurntPercentage => durabilityManager.BurntPercentage;
    public DoraData DoraData => doraData;

    public void InitializeDoraCob(DoraAbstractController i_controller, DoraCellFactory i_cellfactory, SpawnPool i_vfxPool, BoxCollider i_cullingBounds, BoxCollider i_selectionBounds, DoraBatchData i_parentBatch, int i_batchCount, bool i_canSpawnSuper, out bool o_superKernelSpawned)
    {
        fetchData(i_parentBatch);
        cullingBounds = i_cullingBounds;
        selectionBounds = i_selectionBounds;
        populateMap(i_controller, i_cellfactory, i_vfxPool);
        durabilityManager.InitializeKernelDurability(i_canSpawnSuper, i_batchCount, out o_superKernelSpawned);
        RevealCells(false);
    }

    public void ReleaseDoraCob(DoraCellFactory i_cellfactory)
    {
        if (null == i_cellfactory) return;
        if (null == cellMap) return;

        for (int i = 0; i < NB_ROWS; i++)
        {
            for (int j = 0; j < NB_COLUMNS; j++)
            {
                i_cellfactory.ReleaseCell(cellMap[i, j], kernelSpawner);
            }
        }

        cells = null;
        cellMap = null;
        if (null != cellsByGo) cellsByGo.Clear();
        cellsByGo = null;
    }

    public void RevealCells(bool i_animated)
    {
        if (null == cellMap) return;

        if (true == i_animated)
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

    public void EnableRenderers(bool i_enable)
    {
        if (cellMap != null)
        {
            int length0 = CellMapLength0;
            int length1 = CellMapLength1;

            for (int i = 0; i < length0; i++)
            {
                for (int j = 0; j < length1; j++)
                {
                    cellMap[i, j].EnableKernelRenderer(i_enable);
                }
            }
        }

        cobRnd.enabled = i_enable;
    }
    #endregion

    #region PRIVATE

    public void populateMap(DoraAbstractController i_controller, DoraCellFactory i_cellfactory, SpawnPool i_vfxPool)
    {
        if (kernelSpawner == null)
        {
            Debug.LogError("Kernel spawner is null...Cannot populate map!");
            return;
        }

        if (null == i_cellfactory) return;

        int count = anchors.Length;

        cells = new DoraCellData[count];
        cellsByGo = new Dictionary<GameObject, DoraCellData>(count);

        for (int i = 0; i < count; i++)
        {
            cells[i] = i_cellfactory.MakeCell(i_controller, i_vfxPool, kernelSpawner, anchors[i]);
        }

        cellMap = CollectionUtilities.Make2DArray<DoraCellData>(cells, NB_ROWS, NB_COLUMNS);
        DoraCellData currData = null;

        for (int i = 0; i < NB_ROWS; i++)
        {
            for (int j = 0; j < NB_COLUMNS; j++)
            {
                currData = cellMap[i, j];
                currData.SetCoords(new Vector2Int(i, j));
                currData.SetKernelName(i + "," + j);
                cellsByGo.Add(currData.Kernel.gameObject, currData);
            }
        }
    }

    private void fetchData(DoraBatchData i_parentBatch)
    {
        doraData = i_parentBatch.AssignedDoraData;
        durabilityManager.SetBatchData(i_parentBatch);
    }

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
