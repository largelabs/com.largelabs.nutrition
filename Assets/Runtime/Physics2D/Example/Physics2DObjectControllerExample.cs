using UnityEngine;

public class Physics2DObjectControllerExample : MonoBehaviourBase
{
    [SerializeField] PhysicsBody2D body = null;
    [SerializeField] float maxVelocityX = 5;
    [SerializeField] float midAirVelocityX = 2;
    [SerializeField] float maxVelocityY = 10;

    void Update()
    {
        if (null == body) return;

        body.SetVelocityX(0f);

        float horiz = Input.GetAxisRaw("Horizontal");
        body.SetVelocityX((body.IsGrounded ? maxVelocityX : midAirVelocityX) * horiz);

        if (true == body.IsGrounded)
        {
            if (true == Input.GetKeyDown(KeyCode.Space))
                body.SetVelocityY(maxVelocityY);
        }
    }
}
