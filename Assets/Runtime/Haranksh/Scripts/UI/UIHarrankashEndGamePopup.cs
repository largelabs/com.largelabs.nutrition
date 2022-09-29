using UnityEngine;

public class UIHarrankashEndGamePopup : UIEndGamePopup
{
    [SerializeField] private Controls popupInputs = null;
    [SerializeField] private GameObject sparklesContainer = null;

    UIImageFrameSwapper[] sparkles = null;
    protected override void enableInputs()
    {
        popupInputs.EnableControls();
        popupInputs.MoveStarted += onMove;
        popupInputs.Move += onMove;
        popupInputs.JumpPressed += onAction;
    }

    protected override void disableInputs()
    {
        popupInputs.DisableControls();
        popupInputs.MoveStarted -= onMove;
        popupInputs.Move -= onMove;
        popupInputs.JumpPressed -= onAction;
    }
    protected override void onDidAppear()
    {
        base.onDidAppear();
        if (sparkles == null) sparkles = GetComponentsInChildren<UIImageFrameSwapper>();

        foreach (UIImageFrameSwapper sparkle in sparkles)
            sparkle.Play();
    }

    protected override void onDidDisappear()
    {
        base.onDidDisappear();

        if (sparkles == null) return;

        foreach (UIImageFrameSwapper sparkle in sparkles)
            sparkle.Stop();
    }


    #region PUBLIC API

    [ExposePublicMethod]
    public override void Disappear(bool i_animated)
    {
        if (false == IsAppearInit) return;
        disableInputs();
        base.Disappear(i_animated);
    }

    #endregion
}
