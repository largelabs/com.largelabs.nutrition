using UnityEngine;
using UnityEngine.UI;

public class UIDoraEndGamePopup : MonoBehaviourBase, IAppear
{
    [SerializeField] GameObject endGameParticlesRT = null;
    [SerializeField] InterpolatorsManager interpolators = null;
    [SerializeField] AppearWithLocalScale scaleAppear = null;
    [SerializeField] UIAppearWithCanvasGroupAlpha alphaAppear = null;
    [SerializeField] UIAppearWithCanvasGroupAlpha blackBgAppear = null;


    [SerializeField] Text scoreText = null;
    [SerializeField] UIDoraEndGameButton button0 = null;
    [SerializeField] UIDoraEndGameButton button1 = null;
    [SerializeField] DoraInputs popupInputs = null;

    UIDoraEndGameButton selectedButton = null;

    #region UNITY AND CORE

    void OnDestroy()
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
    public void Appear(bool i_animated)
    {
        alphaAppear.Init(interpolators);
        scaleAppear.Init(interpolators);
        blackBgAppear.Init(interpolators);

        scaleAppear.Appear(i_animated);
        alphaAppear.Appear(i_animated);
        blackBgAppear.Appear(i_animated);

        endGameParticlesRT.SetActive(true);

        scaleAppear.OnDidAppear += onDidAppear;
    }

    [ExposePublicMethod]
    public void Disappear(bool i_animated)
    {
        if (false == IsAppearInit) return;

        scaleAppear.OnDidDisappear += onDidDisappear;

        disableInputs();

        scaleAppear.Disappear(i_animated);
        alphaAppear.Disappear(i_animated);
        blackBgAppear.Disappear(i_animated);

    }


    #endregion

    #region PRIVATE

    void onDidAppear()
    {
        scaleAppear.OnDidAppear -= onDidAppear;
        selectButton(button0);
        enableInputs();
    }

    void onDidDisappear()
    {
        endGameParticlesRT.SetActive(false);
        scaleAppear.OnDidDisappear -= onDidDisappear;

        if(null != selectedButton)
        {
            selectedButton.Unselect(true);
            selectedButton = null;
        }
    }

    UIDoraEndGameButton getOtherButton(UIDoraEndGameButton i_button)
    {
        return i_button == button0 ? button1 : button0;
    }

    void selectButton(UIDoraEndGameButton i_button)
    {
        UIDoraEndGameButton otherButton = getOtherButton(i_button);

        if(null != otherButton) otherButton.Unselect(true);
        selectedButton = i_button;
        selectedButton.Select(true);
    }

    void enableInputs()
    {
        popupInputs.EnableInputs();
        popupInputs.OnMoveStarted += onMove;
        popupInputs.OnMove += onMove;
        popupInputs.OnEatStarted += onAction;
    }

    void disableInputs()
    {
        popupInputs.DisableInputs();
        popupInputs.OnMoveStarted -= onMove;
        popupInputs.OnMove -= onMove;
        popupInputs.OnEatStarted -= onAction;
    }

    void onAction()
    {
        if (null == selectedButton) return;

        Disappear(true);
        selectedButton.TriggerCallback();
    }

    void onMove(Vector2 i_move)
    {
        selectButton(getOtherButton(selectedButton));
    }

    #endregion
}
