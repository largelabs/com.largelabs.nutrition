using UnityEngine;

public class DoraKernel : MonoBehaviourBase, ISelectable, IAppear
{
    [SerializeField] DoraKernelAppear appear = null;
    [SerializeField] MeshRenderer kernelRnd = null;

    [Header("Selected materials")]
    [SerializeField] Material kernelMat0Selected = null;
    [SerializeField] Material kernelMat1Selected = null;
    [SerializeField] Material kernelMat2Selected = null;
    [SerializeField] Material kernelMatBurntSelected = null;

    [Header("Unselected materials")]
    [SerializeField] Material kernelMat0 = null;
    [SerializeField] Material kernelMat1 = null;
    [SerializeField] Material kernelMat2 = null;
    [SerializeField] Material kernelMatBurnt = null;

    bool isInit = false;
    bool isBurnt = false;
    float durability = 1f;
    bool isSelected = false;

    #region PUBLIC API

    public void Init(InterpolatorsManager i_interpolators)
    {
        if (true == isInit) return;

        gameObject.SetActive(false);
        appear.Init(i_interpolators);
        isInit = true;
    }

    public void ResetValues()
    {
        isBurnt = false;
        durability = 1f;
        Unselect();
        swapMaterials(durability, isSelected);
    }

    public bool IsInit => isInit;

    public float Durability => durability;

    public bool IsBurnt => isBurnt;

    public void SetDurability(float i_durability)
    {
        durability = Mathf.Clamp01(i_durability);
    }

    public void DecreaseDurability(float i_durability)
    {
        durability -= i_durability;
        durability = Mathf.Clamp01(durability);
    }

    public void IncreaseDurability(float i_durability)
    {
        durability += i_durability;
        durability = Mathf.Clamp01(durability);
    }

    public void SetBurntStatus(bool i_burnt)
    {
        isBurnt = i_burnt;
    }

    public void UpdateColor()
    {
        if (durability == 0f)
        {
            if(isBurnt == false)
                BurnKernel();
        }
        else
        {
            isBurnt = false;
            swapMaterials(durability, isSelected);
        }
    }

    public void BurnKernel()
    {
        isBurnt = true;
        durability = 0f;
        swapMaterials(durability, isSelected);
    }

    public Bounds RendererBounds => kernelRnd.bounds;

    public void EnableRenderer(bool i_enable)
    {
        if (kernelRnd != null)
            kernelRnd.enabled = i_enable;
    }

    #endregion

    #region IAppear

    [ExposePublicMethod]
    public void Appear(bool i_animated) { appear?.Appear(i_animated); }

    [ExposePublicMethod]
    public void Disappear(bool i_animated) { appear?.Disappear(i_animated); }

    public bool IsAppearing => null != appear ? appear.IsAppearing : false;

    public bool IsDisappearing => null != appear ? appear.IsDisappearing : false;

    #endregion

    #region ISelectable

    public bool IsSelected => isSelected;

    public void Select()
    {
        if (true == isSelected) return;
        isSelected = true;
        transform.localScale = MathConstants.VECTOR_3_ONE * 1.2f;
        swapMaterials(durability, isSelected);
    }

    public void Unselect()
    {
        if (false == isSelected) return;
        isSelected = false;
        transform.localScale = MathConstants.VECTOR_3_ONE;
        swapMaterials(durability, isSelected);
    }

    #endregion

    #region PRIVATE

    void swapMaterials(float i_durability, bool i_isSelected)
    {
        if (i_durability == 0f) kernelRnd.material = i_isSelected ? kernelMatBurntSelected : kernelMatBurnt;
        else if (i_durability > 0f && i_durability < 0.25f) kernelRnd.material = i_isSelected ? kernelMat2Selected : kernelMat2;
        else if (i_durability > 0.25f && i_durability < 0.5f) kernelRnd.material = i_isSelected ? kernelMat1Selected : kernelMat1;
        else kernelRnd.material = i_isSelected ? kernelMat0Selected : kernelMat0;
    }


    #endregion
}
