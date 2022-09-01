using System.Collections;
using UnityEngine;

public class DoraKernel : MonoBehaviourBase, ISelectable, IAppear
{
    [SerializeField] DoraKernelAppear appear = null;
    [SerializeField] MeshRenderer kernelRnd = null;
    [SerializeField] Color baseColor = Color.white;
    [SerializeField] Color targetColor = Color.white;
    [SerializeField] Color burntColor = Color.black;

    private static readonly int baseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int firstShadeColorId = Shader.PropertyToID("_1st_ShadeColor");
    private static readonly int secondShadeColorId = Shader.PropertyToID("_2nd_ShadeColor");

    bool isInit = false;
    bool isBurnt = false;
    float durability = 1f;
    bool isSelected = false;

    InterpolatorsManager interpolators = null;

    Material mat = null;
    Color currentColor = Color.white;
    Color currentRenderedColor = Color.white;

    #region UNITY AND CORE

    protected override void Awake()
    {
        base.Awake();

        if(kernelRnd != null)
            mat = kernelRnd.material;
    }

    private void OnDisable()
    {
        if (mat != null)
            mat.SetColor(baseColorId, baseColor);
    }

    private void OnDestroy()
    {
        if (mat != null)
            mat.SetColor(baseColorId, baseColor);
    }

    #endregion

    #region PUBLIC API

    public void Init(InterpolatorsManager i_interpolators)
    {
        if (true == isInit) return;

        interpolators = i_interpolators;
        gameObject.SetActive(false);
        appear.Init(i_interpolators);
        isInit = true;
    }

    public void ResetValues()
    {
        isBurnt = false;
        durability = 1f;
        onDidUnselect(null);
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
            if (mat != null)
            {
                currentColor = Color.Lerp(baseColor, targetColor, (1 - durability));
                setKernelColor(currentColor);
            }
        }
    }

    public void BurnKernel()
    {
        currentColor = Color.black;
        setKernelColor(currentColor);
        isBurnt = true;
        durability = 0f;
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
        setKernelColor(Color.red * currentColor);
    }

    public void Unselect()
    {
        if (false == isSelected) return;
        isSelected = false;

        ITypedAnimator<Color> colorInterpolator = interpolators.Animate(currentRenderedColor, currentColor, 0.15f, new AnimationMode(AnimationType.Ease_In_Out), true, 0f, onDidUnselect);
        animateColorRoutine = StartCoroutine(animateColor(colorInterpolator));

        transform.localScale = MathConstants.VECTOR_3_ONE;
    }

    #endregion

    #region PRIVATE

    Coroutine animateColorRoutine = null;

    IEnumerator animateColor(ITypedAnimator<Color> i_interpolator)
    {
        while(true == i_interpolator.IsAnimating)
        {
            setKernelColor(i_interpolator.Current);
            yield return null;
        }
    } 

    void onDidUnselect(ITypedAnimator<Color> i_interpolator)
    {
        this.DisposeCoroutine(ref animateColorRoutine);

        transform.localScale = MathConstants.VECTOR_3_ONE;
        setKernelColor(currentColor);
    }

    private void setKernelColor(Color i_color)
    {
        if (kernelRnd == null || kernelRnd.enabled == false) return;

        if (mat == null)
        {
            if (kernelRnd != null)
                mat = kernelRnd.material;

            if (mat == null) return;
        }

        currentRenderedColor = i_color;

        mat.SetColor(baseColorId, i_color);
        mat.SetColor(firstShadeColorId, i_color);
        mat.SetColor(secondShadeColorId, i_color);
    }

    #endregion
}
