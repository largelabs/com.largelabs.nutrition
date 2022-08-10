// NEEDS TO BE POOLED
using UnityEngine;

public class DoraCellData
{
    Transform anchor = null;
    DoraKernel kernel = null;
    Vector2Int coords;

    public void Init(DoraKernel i_kernel, Transform i_anchor)
    {
        kernel = i_kernel;
        anchor = i_anchor;
    }

    public void Reset()
    {
        kernel = null;
    }

    public Vector2Int Coords => coords;

    public int X => coords.x;

    public int Y => coords.y;

    public Transform Anchor => anchor;

    public bool HasKernel => null != kernel;

    public DoraKernel Kernel => kernel;

    public void SetCoords(Vector2Int i_coords)
    {
        coords = i_coords;
    }

    public void SetKernelName(string i_name)
    {
        if (null != kernel) kernel.gameObject.name = i_name;
    }
}