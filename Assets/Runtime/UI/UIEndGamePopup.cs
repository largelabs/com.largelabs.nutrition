using UnityEngine;
using UnityEngine.UI;

public class UIEndGamePopup : MonoBehaviourBase, IAppear
{
    [SerializeField] InterpolatorsManager interpolators = null;
    [SerializeField] AppearWithLocalScale scaleAppear = null;
    [SerializeField] UIAppearWithCanvasGroupAlpha alphaAppear = null;
    [SerializeField] UIAppearWithCanvasGroupAlpha blackBgAppear = null;
    [SerializeField] Text scoreText = null;
    [SerializeField] UIEndGameButton button0 = null;
    [SerializeField] UIEndGameButton button1 = null;
    [SerializeField] AudioSource onAppearSfx = null;

    UIEndGameButton selectedButton = null;

    #region UNITY AND CORE

    protected virtual void OnDestroy()
    {
        scaleAppear.OnDidAppear -= onDidAppear;
        scaleAppear.OnDidDisappear -= onDidDisappear;
        disableInputs();
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

    #region PRIVATE


    UIEndGameButton getOtherButton(UIEndGameButton i_button)
    {
        return i_button == button0 ? button1 : button0;
    }

    void selectButton(UIEndGameButton i_button, bool i_animated)
    {
        UIEndGameButton otherButton = getOtherButton(i_button);

        if (null != otherButton) otherButton.Unselect(i_animated);
        selectedButton = i_button;
        selectedButton.Select(i_animated);
    }
    #endregion

    #region PROTECTED

    protected void onAction()
    {
        if (null == selectedButton) return;

        Disappear(true);
        selectedButton.TriggerCallback();
    }

    protected void onMove(Vector2 i_move)
    {
        selectButton(getOtherButton(selectedButton), true);
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

        if (null != selectedButton)
        {
            selectedButton.Unselect(true);
            selectedButton = null;
        }
    }

    protected virtual void enableInputs() { }

    protected virtual void disableInputs() { }

    #endregion

}
