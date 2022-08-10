using UnityEngine;

public class S_idle : State
{
    [SerializeField] PhysicsBody2D body;

    #region PROTECTED
    protected override void onStateInit()
    {
        Debug.Log("Idle State initialized");
    }

    protected override void onStateEnter()
    {
        Debug.Log("Entered IDLE");
        body.SetVelocityX(0f);
        body.SetVelocityY(0f);
        controls.JumpPressed += onJump;
    }

    protected override void onStateExit()
    {
        Debug.Log("Exited IDLE");
        if (body == null)
        {
            Debug.LogError("NO PHYSICS IN STATES");
            return;
        }
        
        controls.JumpPressed -= onJump;
    }

    protected override void onStateUpdate()
    {
        checkFall();
    }

    #endregion

    #region PRIVATE

    private void onJump(){
        setState<S_jump>();
    }

    void checkFall()
    {
        if (false == enabled) return;

        if (false == body.IsGrounded && body.VelocityY < -0.1f)
        {
            setState<S_fall>();
        }
    }

    #endregion
}
