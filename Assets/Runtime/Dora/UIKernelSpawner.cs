using PathologicalGames;
using System.Collections.Generic;
using UnityEngine;

public class UIKernelSpawner : MonoBehaviourBase
{
    [SerializeField] private SpawnPool uiKernelPool = null;

    private static readonly string UIKERNEL = "UIKernel";
    private static readonly string UIKERNELBURNT = "UIKernelBurnt";

    private List<UIDoraKernel> livingKernels = null;

    #region UNITY AND CORE
    protected override void Awake()
    {
        base.Awake();
        livingKernels = new List<UIDoraKernel>();
    }
    #endregion

    #region PUBLIC API
    public IReadOnlyList<UIDoraKernel> LivingKernels => livingKernels;

    [ExposePublicMethod]
    public UIDoraKernel SpawnUIKernelAtAnchor(RectTransform i_anchor, float i_xOffset, bool i_isBurnt)
    {
        if (uiKernelPool == null)
        {
            Debug.LogError("Dora_Kernel pool is null!");
            return null;
        }



        Transform tr = i_isBurnt? uiKernelPool.Spawn(UIKERNELBURNT):uiKernelPool.Spawn(UIKERNEL);
        Transform originalParent = tr.parent;


        tr.SetParent(i_anchor);
        tr.localPosition = new Vector3(i_xOffset, 0, 0);
        tr.localRotation = MathConstants.QUATERNION_IDENTITY;
        tr.localScale = MathConstants.VECTOR_3_ONE;

        tr.SetParent(originalParent);

        UIDoraKernel ret = tr.GetComponent<UIDoraKernel>();

        if (ret != null)
        {
            livingKernels.Add(ret);
        }
        else
            Debug.LogError("No DoraKernel attached to kernel prefab!");

        return ret;
    } 

    public void DespawnKernel(UIDoraKernel i_uiKernel)
    {
        despawnKernel(i_uiKernel);
        livingKernels.Remove(i_uiKernel);
    }

    [ExposePublicMethod]
    public void DespawnAllKernels()
    {
        if (uiKernelPool == null) return;
        if (livingKernels == null || livingKernels.Count < 1) return;

        int length = livingKernels.Count;
        for (int i = 0; i < length; i++)
        {
            despawnKernel(livingKernels[i]);
        }

        livingKernels.Clear();
    }
    #endregion

    #region PRIVATE API
    private void despawnKernel(UIDoraKernel i_uiKernel)
    {
        i_uiKernel.Reset();
        uiKernelPool?.Despawn(i_uiKernel.transform);
    }

    #endregion
}
