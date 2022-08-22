
public class S_fall : FallAbstractState
{
    #region PROTECTED
    protected override void onStateInit()
    {
    }
    protected override void onStateEnter()
    {
        controls.JumpPressed += goToFastFall;
    }

    protected override void onStateExit()
    {
        controls.JumpPressed -= goToFastFall;
    }

    #endregion

    #region PRIVATE

    void goToFastFall()
    {
        setState<S_fastFall>();
    }

    #endregion
}
