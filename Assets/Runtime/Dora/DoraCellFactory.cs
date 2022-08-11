using UnityEngine;

public class DoraCellFactory
{
    InterpolatorsManager interpolators = null;

    public DoraCellFactory(InterpolatorsManager i_interpolators)
    {
        interpolators = i_interpolators;
    }

    public DoraCellData MakeCell(Transform i_anchor, GameObject i_kernalPrefab, int i_index)
    {
        // TODO : take from prefab pool
        GameObject go = GameObject.Instantiate(i_kernalPrefab);
        go.transform.SetParent(i_anchor);
        go.transform.localPosition = MathConstants.VECTOR_3_ZERO;
        go.transform.localRotation = MathConstants.QUATERNION_IDENTITY;
        go.transform.localScale = MathConstants.VECTOR_3_ONE;

        DoraKernel kernel = go.GetComponent<DoraKernel>();
        kernel.Init(interpolators);
        kernel.Disappear(false);

        // TODO :  Take from pool
        DoraCellData cellData = new DoraCellData();
        cellData.Init(kernel, i_anchor);

        return cellData;
    }
}