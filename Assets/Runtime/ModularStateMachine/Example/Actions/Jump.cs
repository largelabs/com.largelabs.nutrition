using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : Action
{
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] float jumpSpeed;
    public override void PerformAction()
    {
        rigidbody.velocity += jumpSpeed * Vector3.up;
    }
}
