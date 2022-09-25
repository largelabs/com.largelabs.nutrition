using UnityEngine;

public class UIDoraEndGamePopup : UIEndGamePopup
{
    [SerializeField] GameObject endGameParticlesRT = null;
    [SerializeField] DoraInputs popupInputs = null;

    #region PROTECTED API
    protected override void enableInputs()
    {
        popupInputs.EnableInputs();
        popupInputs.OnMoveStarted += onMove;
        popupInputs.OnMove += onMove;
        popupInputs.OnEatStarted += onAction;
    }

    protected override void disableInputs()
    {
        popupInputs.DisableInputs();
        popupInputs.OnMoveStarted -= onMove;
        popupInputs.OnMove -= onMove;
        popupInputs.OnEatStarted -= onAction;
    }

    protected override void onDidDisappear()
    {
        endGameParticlesRT.SetActive(false);
        base.onDidDisappear();
    }

    #endregion

    #region PUBLIC API

    [ExposePublicMethod]
    public override void Appear(bool i_animated)
    {
        endGameParticlesRT.SetActive(true);
        base.Appear(i_animated);
    }

    [ExposePublicMethod]
    public override void Disappear(bool i_animated)
    {
        if (false == IsAppearInit) return;
        disableInputs();
        base.Disappear(i_animated);
    }

    #endregion

}
