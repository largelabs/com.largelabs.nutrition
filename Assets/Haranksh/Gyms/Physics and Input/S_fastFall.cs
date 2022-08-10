using UnityEngine;

public class S_fastFall : FallAbstractState
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
