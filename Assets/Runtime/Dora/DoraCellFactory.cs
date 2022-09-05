using PathologicalGames;
using UnityEngine;

public class DoraCellFactory
{
    InterpolatorsManager interpolators = null;

    public DoraCellFactory(InterpolatorsManager i_interpolators)
    {
        interpolators = i_interpolators;
    }

    public DoraCellData MakeCell(DoraKernel i_kernel, SpawnPool i_vfxPool)
    {
        i_kernel.Init(interpolators, i_vfxPool);
        i_kernel.Disappear(false);

        DoraCellData cellData = new DoraCellData();
        cellData.Init(i_kernel, i_kernel.transform.parent);

        return cellData;
    }
}