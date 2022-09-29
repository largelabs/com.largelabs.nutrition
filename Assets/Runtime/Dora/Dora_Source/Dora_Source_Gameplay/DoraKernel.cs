using PathologicalGames;
using UnityEngine;

public enum KernelStatus
{
    Super,
    Normal,
    Burnt
}

public class DoraKernel : MonoBehaviourBase, ISelectable, IAppear
{
    [SerializeField] DoraKernelVFX kernelVFX = null;
    [SerializeField] DoraKernelAppear appear = null;
    [SerializeField] MeshRenderer kernelRnd = null;
    [SerializeField] Collider kernelCollider = null;

    [SerializeField] AnimationCurve selectScaleCurve = null;
    [SerializeField] Vector3 selectedScale = MathConstants.VECTOR_3_ONE;
    [SerializeField] float selectAnimationSpeed = 1f;
    [SerializeField] AnimationCurve unselectScaleCurve = null;
    [SerializeField] float unselectAnimationSpeed = 1f;

    [Header("Selected materials")]
    [SerializeField] Material kernelMat0Selected = null;
    [SerializeField] Material kernelMat1Selected = null;
    [SerializeField] Material kernelMatBurntSelected = null;
    [SerializeField] Material kernelMatSuperSelected = null;

    [Header("Unselected materials")]
    [SerializeField] Material kernelMat0 = null;
    [SerializeField] Material kernelMat1 = null;
    [SerializeField] Material kernelMatBurnt = null;
    [SerializeField] Material kernelMatSuper = null;

    float durability = 1f;
    bool isSelected = false;
    KernelStatus status = KernelStatus.Normal;
    DoraAbstractController controller = null;

    bool canBurn = false;

    #region PUBLIC API

    public KernelStatus Status => status;

    public bool IsBurnable => canBurn && status != KernelStatus.Super;

    public void Init(DoraAbstractController i_controller, InterpolatorsManager i_interpolators, SpawnPool i_vfxPool)
    {
        controller = i_controller;
        appear.Init(i_interpolators);
        kernelVFX.Init(i_vfxPool, i_interpolators);
    }

    public void ResetValues()
    {
        status = KernelStatus.Normal;
        durability = 1f;
        Unselect(false);
        swapMaterials(durability, isSelected);
        kernelVFX.ResetVFX();
    }

    public bool CanDespawn => kernelVFX.HasLiveVFX;

    public float Durability => durability;

    public void SetBurnable(bool i_burnable)
    {
        canBurn = i_burnable;
    }

    public void SetSuper()
    {
        status = KernelStatus.Super;
        SetBurnable(false);
    }

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

    public void UpdateColor()
    {
        // make super kernel unburnable + no durability decrease
        if (durability == 0f)
        {
            if(IsBurnable && status != KernelStatus.Burnt)
                BurnKernel();
        }
        else
        {
            swapMaterials(durability, isSelected);
        }
    }

    public void BurnKernel()
    {
        status = KernelStatus.Burnt;
        durability = 0f;
        swapMaterials(durability, isSelected);
    }

    public Bounds RendererBounds => kernelRnd.bounds;

    public void EnableRenderer(bool i_enable)
    {
        if (kernelRnd != null)
            kernelRnd.enabled = i_enable;
    }

    public void EnableCollider(bool i_enable)
    {
        if (null != kernelCollider)
            kernelCollider.enabled = i_enable;
    }

    public void GetEaten()
    {
        EnableRenderer(false);
        kernelVFX.PlayEatVFX(durability, status == KernelStatus.Burnt);
    }

    #endregion

    #region IAppear

    public bool IsAppearInit => appear.IsAppearInit;

    [ExposePublicMethod]
    public void Appear(bool i_animated) { appear?.Appear(i_animated); }

    [ExposePublicMethod]
    public void Disappear(bool i_animated) { appear?.Disappear(i_animated); }

    public bool IsAppearing => null != appear ? appear.IsAppearing : false;

    public bool IsDisappearing => null != appear ? appear.IsDisappearing : false;

    #endregion

    #region ISelectable

    public bool IsSelected => isSelected;

    public void Select(bool i_animated)
    {
        if (true == isSelected) return;
        isSelected = true;

        swapMaterials(durability, isSelected);

        if (status == KernelStatus.Burnt && false == controller.IsInFrenzy) kernelVFX.PlaySmokeVFX();

        if (true == i_animated)
            kernelVFX.PlayScaleAnimation(selectedScale, selectScaleCurve, selectAnimationSpeed);
        else
        {
            kernelVFX.StopScaleAnimation();
            transform.localScale = selectedScale;
        }
    }

    public void Unselect(bool i_animated)
    {
        if (false == isSelected) return;
        isSelected = false;

        swapMaterials(durability, isSelected);

        if (true == i_animated)
            kernelVFX.PlayScaleAnimation(MathConstants.VECTOR_3_ONE, unselectScaleCurve, unselectAnimationSpeed);
        else
        {
            kernelVFX.StopScaleAnimation();
            transform.localScale = MathConstants.VECTOR_3_ONE;
        }

    }

    #endregion

    #region PRIVATE

    void swapMaterials(float i_durability, bool i_isSelected)
    {
        if(status == KernelStatus.Super) kernelRnd.material = i_isSelected ? kernelMatSuperSelected : kernelMatSuper;
        else if (status == KernelStatus.Burnt) kernelRnd.material = i_isSelected ? kernelMatBurntSelected : kernelMatBurnt;
        else
        {
            if (i_durability < 0.5f) kernelRnd.material = i_isSelected ? kernelMat1Selected : kernelMat1;
            else kernelRnd.material = i_isSelected ? kernelMat0Selected : kernelMat0;
        } 
    }

    #endregion
}
