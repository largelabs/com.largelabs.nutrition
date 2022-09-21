using UnityEngine;

public interface IDoraCellProvider
{
    int CellMapLength0 { get; }
    int CellMapLength1 { get; }

    DoraCellData GetCell(Vector2Int i_coord, bool i_loopX, bool i_loopY);

    Vector2Int GetLoopedCoord(Vector2Int i_coord, bool i_loopX, bool i_loopY);

    Transform GetRowNormal(int i_index, bool i_loop);

    int GetKernelCount();

    DoraCellData[] AllCells { get; }

    int TotalCellCount { get; }
}