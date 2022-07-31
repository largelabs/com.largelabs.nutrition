using UnityEngine;

public class S_idle : State
{
    [SerializeField] HarankashControls controller;
    [SerializeField] PhysicsBody2D body;
    protected override void onStateEnter()
    {
        Debug.Log("Entered IDLE");
        body.SetVelocityX(0);
        controller.JumpPressed += OnJump;
    }

    protected override void onStateExit()
    {
        Debug.Log("Exited IDLE");
        if (body == null)
        {
            Debug.LogError("NO PHYSICS IN STATES");
            return;
        }
        
        controller.JumpPressed -= OnJump;
    }

    protected override void onStateInit()
    {
        Debug.Log("Idle State initialized");
    }

    protected override void onStateUpdate()
    {
    }

    private void OnJump(){
        setState<S_jump>();
    }
}
