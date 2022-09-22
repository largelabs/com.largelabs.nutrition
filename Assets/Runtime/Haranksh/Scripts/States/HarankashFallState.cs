
public class HarankashFallState : FallAbstractState
{
    #region PROTECTED
    protected override void onStateInit()
    {
    }

    protected override void onStateEnter()
    {
        base.onStateEnter();
        //controls.JumpPressed += goToFastFall;
    }

    protected override void onStateExit()
    {
        base.onStateExit();

        controls.JumpPressed -= goToFastFall;
    }

    #endregion

    #region PRIVATE

    void goToFastFall()
    {
        setState<HarankashFastFallState>();
    }

    #endregion
}
