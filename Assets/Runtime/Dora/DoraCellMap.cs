using System.Collections;
using UnityEngine;

public class DoraCellMap : MonoBehaviourBase
{
    [SerializeField] Transform[] anchors = null;
    [SerializeField] Transform[] normalAnchors = null;
    [SerializeField] GameObject kernel = null;
    [SerializeField] InterpolatorsManager interpolators = null;
    [SerializeField] DoraData doraData = null;

    DoraCellData[,] cellMap = null;
    int currentRowIndex = 0;
    int currentColumnIndex = 0;
    Transform rowNormal = null;
    DoraCellFactory cellFactory = null;

    public DoraData DoraData => doraData;
    public int cellMapLength0 => cellMap.GetLength(0);
    public int cellMapLength1 => cellMap.GetLength(1);

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

    public void RevealCells(bool i_animated)
    {
        if (null == cellMap) return;

        if(true == i_animated)
        {
            StartCoroutine(revealCellsAnimated(0, 6, true));
            StartCoroutine(revealCellsAnimated(6, 12, false));
        }
        else
        {
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 11; j++)
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

        cellMap = CollectionUtilities.Make2DArray<DoraCellData>(cellBuffer, 12, 11);

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

    #region Durability
    public float? GetDurability(int i_index0, int i_index1)
    {
        if(cellMap.GetLength(0) <= i_index0) return null;
        if(cellMap.GetLength(1) <= i_index1) return null;

        return cellMap[i_index0, i_index1].GetDurability();
    }

    public bool? KernelIsBurnt(int i_index0, int i_index1)
    {
        if(cellMap.GetLength(0) <= i_index0) return null;
        if(cellMap.GetLength(1) <= i_index1) return null;

        return cellMap[i_index0, i_index1].KernelIsBurnt();
    }

    public bool SetDurability(int i_index0, int i_index1, float i_durability)
    {
        if(cellMap.GetLength(0) <= i_index0) return false;
        if(cellMap.GetLength(1) <= i_index1) return false;

        return cellMap[i_index0, i_index1].SetDurability(i_durability);
    }

    public bool DecreaseDurability(int i_index0, int i_index1, float i_durability)
    {
        if(cellMap.GetLength(0) <= i_index0) return false;
        if(cellMap.GetLength(1) <= i_index1) return false;

        return cellMap[i_index0, i_index1].DecreaseDurability(i_durability);
    }

    public bool IncreaseDurability(int i_index0, int i_index1, float i_durability)
    {
        if(cellMap.GetLength(0) <= i_index0) return false;
        if(cellMap.GetLength(1) <= i_index1) return false;

        return cellMap[i_index0, i_index1].IncreaseDurability(i_durability);
    }

    public bool SetBurntStatus(int i_index0, int i_index1, bool i_burnt)
    {
        if (cellMap.GetLength(0) <= i_index0) return false;
        if (cellMap.GetLength(1) <= i_index1) return false;

        return cellMap[i_index0, i_index1].SetBurntStatus(i_burnt);
    }

    public bool UpdateColor(int i_index0, int i_index1)
    {
        if (cellMap.GetLength(0) <= i_index0) return false;
        if (cellMap.GetLength(1) <= i_index1) return false;

        return cellMap[i_index0, i_index1].UpdateColor();
    }

    public bool BurnKernel(int i_index0, int i_index1)
    {
        if (cellMap.GetLength(0) <= i_index0) return false;
        if (cellMap.GetLength(1) <= i_index1) return false;

        return cellMap[i_index0, i_index1].BurnKernel();
    }

    #endregion
}
