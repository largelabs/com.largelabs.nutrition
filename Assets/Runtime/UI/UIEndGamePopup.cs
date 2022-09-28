using UnityEngine;
using UnityEngine.UI;

public class UIEndGamePopup : UIDualButtonSelector, IAppear
{
    [SerializeField] InterpolatorsManager interpolators = null;
    [SerializeField] AppearWithLocalScale scaleAppear = null;
    [SerializeField] UIAppearWithCanvasGroupAlpha alphaAppear = null;
    [SerializeField] UIAppearWithCanvasGroupAlpha blackBgAppear = null;
    [SerializeField] Text scoreText = null;

    [SerializeField] AudioSource onAppearSfx = null;

    #region UNITY AND CORE

    protected override void OnDestroy()
    {
        scaleAppear.OnDidAppear -= onDidAppear;
        scaleAppear.OnDidDisappear -= onDidDisappear;

        base.OnDestroy();
    }

    #endregion

    #region PUBLIC API

    public void SetScore(string i_score)
    {
        scoreText.text = i_score;
    }

    #endregion

    #region APPEAR

    public bool IsAppearInit => scaleAppear.IsAppearInit && alphaAppear.IsAppearInit && blackBgAppear.IsAppearInit;

    public bool IsAppearing => scaleAppear.IsAppearing || alphaAppear.IsAppearing || blackBgAppear.IsAppearing;

    public bool IsDisappearing => scaleAppear.IsDisappearing || alphaAppear.IsDisappearing || blackBgAppear.IsDisappearing;

    [ExposePublicMethod]
    public virtual void Appear(bool i_animated)
    {
        alphaAppear.Init(interpolators);
        scaleAppear.Init(interpolators);
        blackBgAppear.Init(interpolators);

        scaleAppear.Appear(i_animated);
        alphaAppear.Appear(i_animated);
        blackBgAppear.Appear(i_animated);

        scaleAppear.OnDidAppear += onDidAppear;
    }

    [ExposePublicMethod]
    public virtual void Disappear(bool i_animated)
    {
        if (false == IsAppearInit) return;

        scaleAppear.OnDidDisappear += onDidDisappear;

        scaleAppear.Disappear(i_animated);
        alphaAppear.Disappear(i_animated);
        blackBgAppear.Disappear(i_animated);

    }

    #endregion



    #region PROTECTED

    protected override void onAction()
    {
        if (false == HasSelectedButton) return;

        Disappear(true);
        base.onAction();
    }


    protected virtual void onDidAppear()
    {
        scaleAppear.OnDidAppear -= onDidAppear;
        selectButton(button0, false);
        enableInputs();
        onAppearSfx?.Play();
    }

    protected virtual void onDidDisappear()
    {
        scaleAppear.OnDidDisappear -= onDidDisappear;
        clearSelection();
    }


    #endregion

}
