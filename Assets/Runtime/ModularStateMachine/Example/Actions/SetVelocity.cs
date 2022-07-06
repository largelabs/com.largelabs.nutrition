using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetVelocity : Action
{
    public new Rigidbody rigidbody;
    public float speed;
    public int direction;
    public override void PerformAction()
    {
        if (rigidbody is null)
            return;

        direction = System.Convert.ToInt32(Input.GetKey(KeyCode.D)) - System.Convert.ToInt32(Input.GetKey(KeyCode.A));
        rigidbody.velocity = new Vector3(direction * speed * Time.deltaTime, rigidbody.velocity.y, rigidbody.velocity.z);
    }
}
