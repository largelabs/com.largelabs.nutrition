using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : Action
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float jumpSpeed;
    public override void PerformAction()
    {
        rb.velocity += jumpSpeed * Vector3.up;
    }
}
