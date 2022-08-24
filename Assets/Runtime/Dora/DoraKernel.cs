using UnityEngine;

public class DoraKernel : MonoBehaviourBase, ISelectable, IAppear
{
    [SerializeField] DoraKernalAppear appear = null;
    [SerializeField] MeshRenderer kernelRnd = null;
    [SerializeField] Color baseColor = Color.white;
    [SerializeField] Color targetColor = Color.white;

    int baseColorId = 0;
    int firstShadeColorId = 0;
    int secondShadeColorId = 0;

    Material mat = null;

    bool materialIdsSet = false;

    private void Awake()
    {
        mat = kernelRnd.material;
        baseColorId = Shader.PropertyToID("_BaseColor");
        firstShadeColorId = Shader.PropertyToID("_1st_ShadeColor");
        secondShadeColorId = Shader.PropertyToID("_2nd_ShadeColor");
        materialIdsSet = true;
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

    bool isInit = false;

    public void Init(InterpolatorsManager i_interpolators)
    {
        if (true == isInit) return;

        gameObject.SetActive(false);
        appear.Init(i_interpolators);
        isInit = true;
    }

    public bool IsInit => isInit;

    #region Durability
    float durability = 1f;
    bool isBurnt = false;

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
        if (durability == 0f && isBurnt == false)
        {
            BurnKernel();
        }
        else
        {
            if (mat != null)
            {
                Color lerpedColor = Color.Lerp(baseColor, targetColor, (1 - durability));

                setKernelColor(lerpedColor);
            }
        }
    }

    public void BurnKernel()
    {
        // play animation

        setKernelColor(Color.black);
        isBurnt = true;
        durability = 0f;
    }

    private void setKernelColor(Color i_color)
    {
        if (mat == null) return;
        if (materialIdsSet == false) return;

        mat.SetColor(baseColorId, i_color);
        mat.SetColor(firstShadeColorId, i_color);
        mat.SetColor(secondShadeColorId, i_color);
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

    public bool IsSelected => throw new System.NotImplementedException();

    public void Select()
    {
        throw new System.NotImplementedException();
    }

    public void Unselect()
    {
        throw new System.NotImplementedException();
    }

    #endregion
}
