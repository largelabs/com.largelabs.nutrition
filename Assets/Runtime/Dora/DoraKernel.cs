using UnityEngine;

public class DoraKernel : MonoBehaviourBase, ISelectable, IAppear
{
    [SerializeField] DoraKernalAppear appear = null;

    bool isInit = false;

    public void Init(InterpolatorsManager i_interpolators)
    {
        if (true == isInit) return;

        gameObject.SetActive(false);
        appear.Init(i_interpolators);
        isInit = true;
    }

    public bool IsInit => isInit;

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
