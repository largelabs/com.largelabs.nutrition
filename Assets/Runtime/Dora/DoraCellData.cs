// NEEDS TO BE POOLED
using UnityEngine;

public class DoraCellData : ISelectable
{
    Transform anchor = null;
    DoraKernel kernel = null;
    Vector2Int coords;
    bool isSelected = false;

    #region ISelectable

    public bool IsSelected => isSelected;

    public void Select()
    {
        if (true == isSelected) return;
        if (null != kernel) kernel.Select();
        isSelected = true;
    }

    public void Unselect()
    {
        if (false == isSelected) return;
        if (null != kernel) kernel.Unselect();
        isSelected = false;
    }

    #endregion

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

    public Bounds CellBounds => null != kernel ? kernel.RendererBounds : new Bounds(anchor.position, new Vector3(0.25f, 0.25f, 0.25f));

    public void SetCoords(Vector2Int i_coords)
    {
        coords = i_coords;
    }

    public void SetKernelName(string i_name)
    {
        if (null != kernel) kernel.gameObject.name = i_name;
    }

    #region Durability
    public float? GetDurability()
    {
        if (HasKernel)
            return kernel.Durability;
        else
            return null;
    }

    public bool? KernelIsBurnt()
    {
        if (HasKernel)
            return kernel.IsBurnt;
        else
            return null;
    }

    public bool SetDurability(float i_durability)
    {
        if (HasKernel)
            kernel.SetDurability(i_durability);
        else
            return false;

        return true;
    }

    public bool DecreaseDurability(float i_durability)
    {
        if (HasKernel)
            kernel.DecreaseDurability(i_durability);
        else
            return false;

        return true;
    }

    public bool IncreaseDurability(float i_durability)
    {
        if (HasKernel)
            kernel.IncreaseDurability(i_durability);
        else
            return false;

        return true;
    }

    public bool SetBurntStatus(bool i_burnt)
    {
        if (HasKernel)
            kernel.SetBurntStatus(i_burnt);
        else
            return false;

        return true;
    }

    public bool UpdateColor()
    {
        if (HasKernel)
            kernel.UpdateColor();
        else
            return false;

        return true;
    }

    public bool BurnKernel()
    {
        if (HasKernel)
            kernel.BurnKernel();
        else
            return false;

        return true;
    }

    #endregion
}