using UnityEngine;

public class HarankashFastFallState : FallAbstractState
{
    #region PROTECTED

    protected override void onStateInit()
    {
    }

    protected override void onStateEnter()
    {
        Debug.Log("Enter fast fall");
    }

    protected override void onStateExit()
    {
    }

    #endregion
}
