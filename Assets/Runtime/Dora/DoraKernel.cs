using PathologicalGames;
using System.Collections;
using UnityEngine;

public class DoraKernel : MonoBehaviourBase, ISelectable, IAppear
{
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

    bool isInit = false;
    bool isBurnt = false;
    float durability = 1f;
    bool isSelected = false;
    bool isMarkedforSelection = false;
    InterpolatorsManager interpolators = null;
    SpawnPool vfxPool = null;


    private static readonly string BURNT_SELECT_VFX_PREFAB = "VFX_Select_Smoke";

    bool isBurnable = false;
    bool isSuper = false;

    #region PUBLIC API
    public void Init(InterpolatorsManager i_interpolators, SpawnPool i_vfxPool)
    {
        if (true == isInit) return;

        interpolators = i_interpolators;
        vfxPool = i_vfxPool;
        gameObject.SetActive(false);
        appear.Init(i_interpolators);
        isInit = true;
    }

    public void ResetValues()
    {
        isBurnt = false;
        durability = 1f;
        Unselect(false);
        swapMaterials(durability, isSelected);
    }

    public bool IsInit => isInit;

    public float Durability => durability;

    public bool IsBurnt => isBurnt;
    public bool IsBurnable => isBurnable;
    public bool IsSuper => isSuper;

    public void SetBurnable(bool i_burnable)
    {
        isBurnable = i_burnable;
    }

    public void SetSuper(bool i_super)
    {
        isSuper = i_super;
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

    public void SetBurntStatus(bool i_burnt)
    {
        isBurnt = i_burnt;
    }

    public void UpdateColor()
    {
        // make super kernel unburnable + no durability decrease
        if (durability == 0f)
        {
            if(isBurnable && isBurnt == false)
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

    public void EnableCollider(bool i_enable)
    {
        if (null != kernelCollider)
            kernelCollider.enabled = i_enable;
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

    Coroutine updateScaleRoutine = null;

    IEnumerator updateScale(ITypedAnimator<Vector3> i_interpolator)
    {
        while(true == i_interpolator.IsAnimating)
        {
            transform.localScale = i_interpolator.Current;
            yield return null;
        }

        this.DisposeCoroutine(ref updateScaleRoutine);
    }

    #region ISelectable

    public bool IsSelected => isSelected;

    public void Select(bool i_animated)
    {
        if (true == isSelected) return;
        isSelected = true;

        swapMaterials(durability, isSelected);

        if (true == i_animated)
            startScaleAnimation(selectedScale, selectScaleCurve, selectAnimationSpeed);
        else
        {
            this.DisposeCoroutine(ref updateScaleRoutine);
            transform.localScale = selectedScale;
        }

        if(true == isBurnt)
        {
            Transform vfxTr = vfxPool.Spawn(BURNT_SELECT_VFX_PREFAB);
            vfxTr.SetParent(transform);
            vfxTr.localScale = MathConstants.VECTOR_3_ONE;
            vfxTr.localPosition = new Vector3(0f, 0f, 0.5f);

            vfxTr.SetParent(null);
            float scaleValue = Mathf.Max(vfxTr.localScale.x, vfxTr.localScale.y);
            vfxTr.localScale = MathConstants.VECTOR_3_ONE * scaleValue;
            vfxTr.SetParent(transform);
        }
    }

    public void Unselect(bool i_animated)
    {
        if (false == isSelected) return;
        isSelected = false;

        swapMaterials(durability, isSelected);

        if (true == i_animated)
            startScaleAnimation(MathConstants.VECTOR_3_ONE, unselectScaleCurve, unselectAnimationSpeed);
        else
        {
            this.DisposeCoroutine(ref updateScaleRoutine);
            transform.localScale = MathConstants.VECTOR_3_ONE;
        }

    }

    public void MarkForSelection(bool i_animated)
    {
        if (true == isMarkedforSelection) return;

        blinkRoutine = StartCoroutine(blink());

        isMarkedforSelection = true;
    }

    Coroutine blinkRoutine = null;

    IEnumerator blink()
    {
        while(true)
        {
            swapMaterials(durability, true);
            yield return this.Wait(0.1f);
            swapMaterials(durability, false);
            yield return this.Wait(0.1f);
        }
    }

    public void UnmarkForSelection(bool i_animated)
    {
        if (false == isMarkedforSelection) return;
        isMarkedforSelection = false;

        this.DisposeCoroutine(ref blinkRoutine);

        swapMaterials(durability, isSelected);

    }

    #endregion

    #region PRIVATE

    void startScaleAnimation(Vector3 i_target, AnimationCurve i_animationCurve, float i_animationSpeed)
    {
        this.DisposeCoroutine(ref updateScaleRoutine);
        float animationTime = Mathf.Abs(transform.localScale.z - i_target.z) / i_animationSpeed;
        ITypedAnimator<Vector3> scaleInterpolator = interpolators.Animate(transform.localScale, i_target, animationTime, new AnimationMode(i_animationCurve), false, 0f, null);

        updateScaleRoutine = StartCoroutine(updateScale(scaleInterpolator));
    }

    void swapMaterials(float i_durability, bool i_isSelected)
    {
        if(isSuper) kernelRnd.material = i_isSelected ? kernelMatSuperSelected : kernelMatSuper;
        else if (isBurnt) kernelRnd.material = i_isSelected ? kernelMatBurntSelected : kernelMatBurnt;
        else
        {
            if (i_durability < 0.5f) kernelRnd.material = i_isSelected ? kernelMat1Selected : kernelMat1;
            else kernelRnd.material = i_isSelected ? kernelMat0Selected : kernelMat0;
        } 
    }


    #endregion
}
