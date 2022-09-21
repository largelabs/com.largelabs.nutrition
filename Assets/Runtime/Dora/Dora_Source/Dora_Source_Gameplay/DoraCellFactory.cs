using PathologicalGames;
using UnityEngine;

public class DoraCellFactory
{
    private InterpolatorsManager interpolators = null;
    private ManagedPool<DoraCellData> cellPool = null;

    public DoraCellFactory(InterpolatorsManager i_interpolators)
    {
        interpolators = i_interpolators;
        cellPool = new ManagedPool<DoraCellData>(-1);
    }

    public DoraCellData MakeCell(SpawnPool i_vfxPool, KernelSpawner i_kernelSpawner, Transform i_anchor)
    {
        DoraKernel kernel = i_kernelSpawner.SpawnDoraKernelAtAnchor(i_anchor);

        kernel.Init(interpolators, i_vfxPool);
        kernel.Disappear(false);

        DoraCellData cellData = cellPool.GetItem();
        cellData.Init(kernel, i_anchor);

        return cellData;
    }

    public void ReleaseCell(DoraCellData i_cell, KernelSpawner i_kernelSpawner)
    {
        if (null == i_cell) return;

        if(true == i_cell.HasKernel)
        {
            i_kernelSpawner?.RequestKernelDespawn(i_cell.Kernel, true);
        }

        cellPool.ResetItem(i_cell);

    }
}