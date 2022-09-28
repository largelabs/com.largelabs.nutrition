using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : UIDualButtonSelector
{
    [SerializeField] DoraInputs menuInputs = null;

    #region UNITY AND CORE

    private void Start()
    {
        selectButton(button0, false);
        enableInputs();
    }

    #endregion


    #region PUBLIC API

    public void OnPressedDora()
    {
        disableInputs();
        SceneManager.LoadScene("Dora");
    }


    public void OnPressedHara()
    {
        disableInputs();
        SceneManager.LoadScene("HarrankashMain");
    }

    #endregion

    #region PROTECTED API

    protected override void enableInputs()
    {
        menuInputs.EnableInputs();
        menuInputs.OnMoveStarted += onMove;
        menuInputs.OnMove += onMove;
        menuInputs.OnEatStarted += onAction;
    }

    protected override void disableInputs()
    {
        menuInputs.DisableInputs();
        menuInputs.OnMoveStarted -= onMove;
        menuInputs.OnMove -= onMove;
        menuInputs.OnEatStarted -= onAction;
    }

    #endregion

}
