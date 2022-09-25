using UnityEngine;

public class UIHarrankashEndGamePopup : UIEndGamePopup
{
    [SerializeField] private Controls popupInputs = null;
    protected override void enableInputs()
    {
        popupInputs.EnableControls();
        popupInputs.OnMoveStarted += onMove;
        popupInputs.OnMove += onMove;
        popupInputs.OnEatStarted += onAction;
    }

    protected override void disableInputs()
    {
        popupInputs.DisableControls();
        popupInputs.OnMoveStarted -= onMove;
        popupInputs.OnMove -= onMove;
        popupInputs.OnEatStarted -= onAction;
    }

}
