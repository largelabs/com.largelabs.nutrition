using UnityEngine;

public class DoraCellData : ISelectable, IPoolable
{
    Transform anchor = null;
    DoraKernel kernel = null;
    Vector2Int coords;
    bool isSelected = false;

    #region IPoolable

    public void Dispose()
    {
        Reset();
    }

    public void Reset()
    {
        Unselect(false);
        kernel = null;
        anchor = null;
    }

    #endregion

    #region ISelectable

    public bool IsSelected => isSelected;

    public void Select(bool i_animated)
    {
        if (true == isSelected) return;
        if (null != kernel) kernel.Select(i_animated);
        isSelected = true;
    }

    public void Unselect(bool i_animated)
    {
        if (false == isSelected) return;
        if (null != kernel) kernel.Unselect(i_animated);
        isSelected = false;
    }

    #endregion

    public void Init(DoraKernel i_kernel, Transform i_anchor)
    {
        kernel = i_kernel;
        anchor = i_anchor;
    }

    public Vector2Int Coords => coords;

    public Transform Anchor => anchor;

    public bool HasKernel => null != kernel;

    public DoraKernel Kernel => kernel;

    public Bounds GetCellBounds()
    {
        if (null != kernel) return kernel.RendererBounds;
        return new Bounds();
    }

    public void SetCoords(Vector2Int i_coords)
    {
        coords = i_coords;
    }

    public void SetKernelName(string i_name)
    {
        if (null != kernel) kernel.gameObject.name = i_name;
    }

    public void EnableKernelRenderer(bool i_enable)
    {
        if (kernel != null)
            kernel.EnableRenderer(i_enable);
    }

    public void EnableKernelCollider(bool i_enable)
    {
        if (kernel != null)
            kernel.EnableCollider(i_enable);
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

    public bool? KernelIsSuper()
    {
        if (HasKernel)
            return kernel.IsSuper;
        else
            return null;
    }

    public bool? KernelIsBurnable()
    {
        if (HasKernel)
            return kernel.IsBurnable;
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
    
    public bool SetSuper(bool i_super)
    {
        if (HasKernel)
            kernel.SetSuper(i_super);
        else
            return false;

        return true;
    }
    
    public bool SetBurnable(bool i_burnable)
    {
        if (HasKernel)
            kernel.SetBurnable(i_burnable);
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