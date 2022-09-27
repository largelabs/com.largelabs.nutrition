using UnityEngine;

public class UIHarrankashEndGamePopup : UIEndGamePopup
{
    [SerializeField] private Controls popupInputs = null;
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

}
