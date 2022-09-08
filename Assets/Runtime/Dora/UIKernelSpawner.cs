using PathologicalGames;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIKernelSpawner : MonoBehaviourBase
{
    [SerializeField] private SpawnPool uiKernelPool = null;

    private static readonly string UIKERNEL = "UIKernel";
    private static readonly string UIKERNELBURNT = "UIKernelBurnt";

    private List<RectTransform> livingKernels = null;

    #region UNITY API
    protected override void Awake()
    {
        base.Awake();

        livingKernels = new List<RectTransform>();
    }
    #endregion

    #region PUBLIC API
    public IReadOnlyList<RectTransform> LivingKernels => livingKernels;

    [ExposePublicMethod]
    public RectTransform SpawnUIKernelAtAnchor(RectTransform i_anchor, float i_xOffset, bool i_isBurnt)
    {
        if (uiKernelPool == null)
        {
            Debug.LogError("Dora_Kernel pool is null!");
            return null;
        }


        Transform tr = i_isBurnt? uiKernelPool.Spawn(UIKERNELBURNT):uiKernelPool.Spawn(UIKERNEL);
        tr.SetParent(i_anchor);
        tr.localPosition = new Vector3(i_xOffset, 0, 0);
        tr.localRotation = MathConstants.QUATERNION_IDENTITY;
        tr.localScale = MathConstants.VECTOR_3_ONE;

        RectTransform ret = tr.GetComponent<RectTransform>();

        if (ret != null)
        {
            livingKernels.Add(ret);
        }
        else
            Debug.LogError("No DoraKernel attached to kernel prefab!");

        return ret;
    } 
    
    [ExposePublicMethod]
    public List<RectTransform> SpawnUIKernelsStartingFromAnchor(RectTransform i_anchor, int i_spawnCount, float i_xOffset, bool i_isBurnt)
    {
        if (uiKernelPool == null)
        {
            Debug.LogError("Dora_Kernel pool is null!");
            return null;
        }

        List<RectTransform> ret = new List<RectTransform>();

        for (int i = 0; i < i_spawnCount; i++)
        {
            ret.Add(SpawnUIKernelAtAnchor(i_anchor, i_xOffset * i, i_isBurnt));
        }

        return ret;
    }

    private Vector3 shiftPositionX(Vector3 i_inputPos, float i_xOffset)
    {
        i_inputPos.x += i_xOffset;
        return i_inputPos;
    }

    public void DespawnKernel(RectTransform i_uiKernel)
    {
        despawnDoraCob(i_uiKernel);
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
            despawnDoraCob(livingKernels[i]);
        }

        livingKernels.Clear();
    }
    #endregion

    #region PRIVATE API
    private void despawnDoraCob(RectTransform i_uiKernel)
    {
        Image kernelImage = i_uiKernel.GetComponent<Image>();
        if (kernelImage != null)
        {
            Color temp = kernelImage.color;
            kernelImage.color = new Color(temp.r, temp.g, temp.b, 1f);
        }
        uiKernelPool?.Despawn(i_uiKernel);
    }

    #endregion
}
