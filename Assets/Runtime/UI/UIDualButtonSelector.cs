using UnityEngine;

public class UIDualButtonSelector : MonoBehaviourBase
{
    [SerializeField] protected UIButton button0 = null;
    [SerializeField] protected UIButton button1 = null;

    UIButton selectedButton = null;

    #region UNITY AND CORE

    protected virtual void OnDestroy()
    {
        disableInputs();
    }

    #endregion

    #region PUBLIC API

    public bool HasSelectedButton => null != selectedButton;

    #endregion

    #region PROTECTED

    protected virtual void onAction()
    {
        if (null == selectedButton) return;
        selectedButton.TriggerCallback();
    }

    protected virtual void onMove(Vector2 i_move)
    {
        selectButton(getOtherButton(selectedButton), true);
    }

    protected virtual void onMove()
    {
        onMove(Vector2.zero);
    }

    protected virtual void enableInputs() { }

    protected virtual void disableInputs() { }

    protected void selectButton(UIButton i_button, bool i_animated)
    {
        UIButton otherButton = getOtherButton(i_button);

        if (null != otherButton) otherButton.Unselect(i_animated);
        selectedButton = i_button;
        selectedButton.Select(i_animated);
    }

    protected void clearSelection()
    {
        if (null == selectedButton) return;

        selectedButton.Unselect(true);
        selectedButton = null;
    }

    #endregion


    #region PRIVATE


    UIButton getOtherButton(UIButton i_button)
    {
        return i_button == button0 ? button1 : button0;
    }


    #endregion

}
